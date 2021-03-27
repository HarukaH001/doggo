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
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Crud");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> Login([Bind("Email,Password")] LoginDTO credential)
        {
            if(User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Crud");
            }
            if (ModelState.IsValid)
            {
                var res = _userService.CookieAuthenticate(credential);

                if (res.Error == false)
                {
                    ClaimsPrincipal claims = res.Data;
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims);
                    return RedirectToAction("Index", "Crud");
                }
                ModelState.AddModelError(string.Empty, "เข้าสู่ระบบไม่สำเร็จ");
            }
            return View(credential);
        }

        [AllowAnonymous]
        [Route("[action]")]
        public IActionResult Register()
        {
            if(User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Crud");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> Register([Bind("Name,Email,Password,ConfirmPassword")] RegisterView credential)
        {
            if(User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Crud");
            }
            if (ModelState.IsValid)
            {
                var res = _userService.SignUpAndAuthenticate(credential);

                if (res.Error == false)
                {
                    ClaimsPrincipal claims = res.Data;
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims);
                    return RedirectToAction("Index", "Crud");
                }
                ModelState.AddModelError(string.Empty, "สมัครสมาชิกไม่สำเร็จ");
            }
            return View(credential);
        }

        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
