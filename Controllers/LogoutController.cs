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

namespace doggo.Controllers
{
    public class LogoutController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("jwt");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("_i");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("_r");
            return RedirectToAction("Index", "Login");
        }
    }
}
