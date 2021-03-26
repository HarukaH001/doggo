using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doggo.Data;
using doggo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace doggo.Controllers
{
    [Authorize]
    public class CrudController : Controller
    {
        private readonly DBContext _context;
        public CrudController(DBContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            Console.WriteLine(User.FindFirst(ClaimTypes.Name).Value);
            var res = ( from u in _context.User
                        orderby u.Id ascending
                        select new UserView{
                            Id = u.Id,
                            Name = u.Name,
                            Email = u.Email,
                            Password = u.Password,
                            CreatedDate = u.CreatedDate,
                            UpdatedDate = u.UpdatedDate,
                            UserRole = u.UserRole
                        });

            return View(await res.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDTO = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDTO == null)
            {
                return NotFound();
            }

            return View(userDTO);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Password,ConfirmPassword")] RegisterView u)
        {
            if (ModelState.IsValid)
            {
                UserDTO user = new UserDTO();
                user.Id = u.Id;
                user.Name = u.Name;
                user.Email = u.Email;
                user.Password = u.Password;
                user.UserRole = "User";
                user.CreatedDate = DateTime.Now;
                user.UpdatedDate = DateTime.Now;
                
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(u);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDTO = await _context.User.FindAsync(id);
            if (userDTO == null)
            {
                return NotFound();
            }
            return View(userDTO);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedDate,UpdatedDate,Email,Password,UserRole")] UserDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userDTO.UpdatedDate = DateTime.Now;
                    _context.Update(userDTO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDTOExists(userDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userDTO);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDTO = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDTO == null)
            {
                return NotFound();
            }

            return View(userDTO);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userDTO = await _context.User.FindAsync(id);
            _context.User.Remove(userDTO);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserDTOExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}