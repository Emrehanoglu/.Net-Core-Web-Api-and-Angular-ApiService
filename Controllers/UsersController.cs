using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Data;

namespace ServerApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public ISocialRepository _socialRepository;

        public UsersController(ISocialRepository socialRepository)
        {
            _socialRepository = socialRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _socialRepository.GetUsers();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _socialRepository.GetUser(id);
            return Ok(user);
        }
    }
}