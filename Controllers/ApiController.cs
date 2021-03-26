using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using doggo.Data;
using doggo.Models;
using doggo.Services;

namespace doggo.Controllers{
    [Authorize]
    public class ApiController : Controller
    {
        private IUserService _userService;

        public ApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<Backpass> authenticate([FromBody] LoginDTO credential)
        {
            var backpass = _userService.Authenticate(credential);
            return backpass;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }
    }
}