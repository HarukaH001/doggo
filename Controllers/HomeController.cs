using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using doggo.Data;
using doggo.Models;
using doggo.Services;

namespace doggo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/[action]/{id}")]
        public IActionResult Info(int id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [Authorize(Roles = "User")]
        [Route("[controller]/[action]/{id}")]
        public IActionResult Reserve(int id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/Info/[action]/{id}")]
        public IActionResult TimeTable(int id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
