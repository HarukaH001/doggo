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
        private readonly IItemService _itemService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IUserService userService, IItemService itemService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _itemService = itemService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var res = await _itemService.GetItems();
            return View(res);
        }

        [Authorize(Roles = "Admin")]
        [Route("[controller]/[action]/{id}")]
        public IActionResult Info(int id)
        {
            ViewData["Id"] = id;

            var res = _itemService.FullItemInfo(id);

            return View(res);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Info(AddDeleteItemDTO value)
        {
            var id = value.Id;
            if(value.Type == "เพิ่มรายการ"){
                await _itemService.AddById(id, value.Val);
            } else {
                await _itemService.DeleteById(id, value.Val);
            }

            return RedirectToAction("Info", new { id = id });
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
        public async Task<IActionResult> TimeTable(int id)
        {
            ViewData["Id"] = id;
            var res = await _itemService.GetReservationByItemId(id);
            return View(res);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("[controller]/Info/[action]/{id}")]
        public async Task<IActionResult> TimeTable(int id, TimeTableView model)
        {
            ViewData["Id"] = id;
            var res = await _itemService.GetReservationByItemId(id, model.ReserveDate);
            return View(res);
        }

        [Authorize(Roles = "User")]
        [Route("[action]")]
        public IActionResult History()
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var data = _itemService.GetHistoryById(userId);
            return View(data);
        }
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var res = await _itemService.DeleteReservationById(id);
            return RedirectToAction("History");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BatchDeleteReservation(int itemId, int userId, DateTime reserveDate, string slots)
        {
            var sls = slots.Split(',').Select(Int32.Parse).ToList();
            Console.WriteLine(itemId);
            Console.WriteLine(userId);
            Console.WriteLine(reserveDate.ToString());
            Console.WriteLine(slots);

            var res = await _itemService.BatchDeleteReservation(itemId, userId, reserveDate, sls);

            return RedirectToAction("TimeTable", new { id = itemId });
        }

        [Route("{*url:regex(^(?!api).*$)}", Order = 999)]
        public IActionResult CatchAll()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
