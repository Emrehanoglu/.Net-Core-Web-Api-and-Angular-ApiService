using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        
        public UserController(UserManager<User> userManager,SignInManager<User> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        //http://localhost:5000/api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO model){
            var user = new User{
                UserName = model.UserName,
                Email = model.Email,
                Name = model.Name,
                Gender = model.Gender,
                Created = DateTime.Now,
                LastActive = DateTime.Now
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Succeeded){
                return StatusCode(201);
            }
            return BadRequest(result.Errors);
        }

        //http://localhost:5000/api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO model){
            try
            {
                throw new Exception("deneme hatası");
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e.Message);
            }

            //model üzerinden UserName bilgisi alındığı için,
            //bakalım gercekten boyle bir kullanıcı var mı? 
            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user==null){
                return BadRequest(new { message = "username is not exists"});
            }

            //username bilgisi ile parolayı kontrol ediyorum
            //bunun için SignInManager gerekiyor
            //lockoutOnFailure:false ile Startup.cs içerisindeki konf. ezerek hesap kitlenmesin diyorum
            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,lockoutOnFailure:false);
            if(result.Succeeded){
                //login
                return Ok(new {
                    token  = GenerateJwtToken(user)
                });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
        
            var tokenDescriptor = new SecurityTokenDescriptor{
                //token bilgisi içerisinde olmasını istediğim kısımlar
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName)
                }),
                //token 'ı kim oluşturdu bilgisi
                Issuer = "emrehanoglu.com",

                //token gecerlilik süresi bilgisi
                Expires = DateTime.UtcNow.AddDays(1),

                //token 'ı şifrelendiği kısım,
                //şifrelenirken kullanılan key bilgisi ve algoritma verilir
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature)
            };

            //artık token 'ı oluşturabilirim.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //elimde byte tipinde token bilgisi var artık bunu string tipinde geri döndürmem lazım
            return tokenHandler.WriteToken(token);
        }
    }
}