using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using ServerApp.DTO;

namespace ServerApp.Controllers
{
    [Authorize]
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
    }
}