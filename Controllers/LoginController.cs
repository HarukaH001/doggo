using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using doggo.Data;
using doggo.Models;

namespace doggo.Controllers
{
    public class LoginController : Controller
    {
        private readonly DBContext db;

        public LoginController(DBContext context)
        {
            db = context;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index([Bind("Email,Password")] LoginDTO credential)
        {
            if (ModelState.IsValid)
            {
                var res = (from u in db.User
                           where u.Email == credential.Email
                           select u);

                if (res.Any())
                {
                    UserDTO user = await res.FirstOrDefaultAsync();
                    if (user.Password == credential.Password)
                    {

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "เข้าสู่ระบบไม่สำเร็จ");
            }
            return View(credential);
        }
    }
}