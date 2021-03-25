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
    public class RegisterController : Controller
    {
        private readonly DBContext db;

        public RegisterController(DBContext context)
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
        public async Task<IActionResult> Index([Bind("Name,Email,Password,ConfirmPassword")] RegisterView credential)
        {
            if (ModelState.IsValid)
            {
                UserDTO user = new UserDTO();
                user.Id = credential.Id;
                user.Name = credential.Name;
                user.Email = credential.Email;
                user.Password = credential.Password;
                user.UserRole = 1;
                user.CreatedDate = DateTime.Now;
                user.UpdatedDate = DateTime.Now;

                db.Add(user);
                await db.SaveChangesAsync();

                var res = ( from u in db.User
                            where u.Email == credential.Email
                            select u );

                if(res.Any()) {
                    UserDTO identity = await res.FirstOrDefaultAsync();
                    if(identity.Password == credential.Password){
                        return RedirectToAction("Index","Crud");
                    }
                }

                ModelState.AddModelError(string.Empty, "สมัครสมาชิกไม่สำเร็จ");
            }
            return View(credential);
        }
    }
}