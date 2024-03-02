using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.Helpers;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    //[ServiceFilter(typeof(LastActiveActionFilter))]
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public ISocialRepository _socialRepository;
        private readonly IMapper _mapper;

        public UsersController(ISocialRepository socialRepository, IMapper mapper)
        {
            _socialRepository = socialRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _socialRepository.GetUsers();

            //hedef tip yani dönüştürmek istediğim tip UserForListDTO,
            //dönüştürülen bilgi ise users.
            //GetUsers metodu IEnumerable geldiği için Map kısmıda IEnumerable olmalı.
            var listOfUsers = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            
            return Ok(listOfUsers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _socialRepository.GetUser(id);

            var userObj = _mapper.Map<UserForDetailsDTO>(user);
            
            return Ok(userObj);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO model)
        {
            //kullanıcının elindeki token bilgisi içerisindeki payload alanındaki id bilgisi ile
            //update metoduna gelen id bilgisinin aynı olması gerekiyor
            //kullanıcı kendisinden başka bir id 'yi güncelleyememeli
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return BadRequest("not valid request");
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _socialRepository.GetUser(id);

            //model 'den gelen bilgiler UserForUpdateDTO içerisine set edilecek.
            _mapper.Map<UserForUpdateDTO>(model);

            if(await _socialRepository.SaveChanges()){
                return Ok();
            }else{
                throw new Exception("güncelleme sırasında bir hata olustu");
            }
        }

        //followerUserId ----> sisteme login olan, takip edecek olan kullanıcı
        //userId ----> takip edilecek olan kullanıcı
        //https://localhost:5000/api/users/1/follow/2 
        [HttpPost("{followerUserId}/follow/{userId}")]
        public async Task<IActionResult> FollowUser(int followerUserId, int userId){
            //sisteme login olan kullanıcının token bilgisi içerisindeki id ile
            //url 'deki followerId aynı olmalı
            if(followerUserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            if(followerUserId == userId){
                return BadRequest("Kendinizi takip edemezsiniz");
            }

            //kullanıcı daha önceden diğer kullanıcıyı takip etmiş mi kontrolü
            var IsAlreadyFollowed = await _socialRepository.IsAlreadyFollowed(followerUserId,userId);
            if(IsAlreadyFollowed){
                return BadRequest("Zaten kullanıcıyı takip ediyorsunuz");
            }

            //takip etmek istenilen kullanıcı bilgisi sistemde gercekten var mı kontrolü
            if(await _socialRepository.GetUser(userId) == null){
                return NotFound();
            }

            //her şey ok ise.
            var follow = new UserToUser(){
                UserId = userId,
                FollowerId = followerUserId
            };

            _socialRepository.Add<UserToUser>(follow);

            if(await _socialRepository.SaveChanges())
                return Ok();
            
            return BadRequest("Hata Oluştu");
        }
    }
}