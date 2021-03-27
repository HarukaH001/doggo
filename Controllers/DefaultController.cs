using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using doggo.Data;
using doggo.Models;
using doggo.Services;

namespace doggo.Controllers{
    [Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[action]")]
    [ApiController]
    public class DefaultController : Controller
    {
        private IUserService _userService;

        public DefaultController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<Backpass> Authenticate([FromBody] LoginView credential)
        {
            var backpass = _userService.Authenticate(credential);
            return backpass;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult users()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }
    }
}