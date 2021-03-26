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
    public class RegisterController : Controller
    {
        private IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(IUserService userService, IHttpContextAccessor httpContextAccessor)
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
        public IActionResult Index([Bind("Name,Email,Password,ConfirmPassword")] RegisterView credential)
        {
            if (ModelState.IsValid)
            {
                var res = _userService.SignUpAndAuthenticate(credential);

                if (res.Error == false)
                {
                    UserDTO user = res.Data.UserInfo;
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", res.Data.Token, new CookieOptions());
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("_i", user.Id.ToString(), new CookieOptions());
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("_r", user.UserRole.Substring(0,1), new CookieOptions());
                    return RedirectToAction("Index", "Crud");
                }
                ModelState.AddModelError(string.Empty, "สมัครสมาชิกไม่สำเร็จ");
            }
            return View(credential);
        }
    }
}