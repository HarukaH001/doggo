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
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Index([Bind("Email,Password")] LoginDTO credential)
        {
            if (ModelState.IsValid)
            {
                var res = _userService.Authenticate(credential);

                if (res.Error == false)
                {
                    UserDTO user = res.Data.UserInfo;
                    // _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", res.Data.Token, new CookieOptions());
                    return RedirectToAction("Index", "Crud");
                    // return View();
                }
                ModelState.AddModelError(string.Empty, "เข้าสู่ระบบไม่สำเร็จ");
            }
            return View(credential);
        }
    }
}
