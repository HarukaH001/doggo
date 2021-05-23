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
using System.Text.Json;
using System.Text.Json.Serialization;

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
            if (value.Type == "เพิ่มรายการ")
            {
                await _itemService.AddById(id, value.Val);
            }
            else
            {
                await _itemService.DeleteById(id, value.Val);
                await _itemService.UpdateReservationRecordById(id);
            }

            return RedirectToAction("Info", new { id = id });
        }


        [Authorize(Roles = "User")]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Reserve(int id)
        {
            ViewData["Id"] = id;
            var (info, item) = _itemService.GetReservationItemToday(id);
            string ww = JsonSerializer.Serialize(info);
            ViewData["ItemReserveInfo"] = ww;
            ViewData["Name"] = item.Name;
            ViewData["Loc"] = item.Location;
            ViewData["User"] = User.FindFirst("Id").Value;

            var timeslot = new List<string>{"09:00-10:00",
"10:00-11:00",
"11:00-12:00",
"12:00-13:00",
"13:00-14:00",
"14:00-15:00",
"15:00-16:00",
"16:00-17:00",
"17:00-18:00",
"18:00-19:00"};
            ViewData["TimeSlot"] = timeslot;
            return View();
        }

        [HttpGet]
        [Route("[controller]/reservation/{id}/{date}")]
        public string reservationFromDate(int id, string date)
        {
            var d = date.Split('-').Select(val => int.Parse(val)).ToArray();
            var target = new DateTime(d[0], d[1], d[2]); // yyyy-mm-dd
            var (info, name) = _itemService.GetReservationItem(id, target);
            string ww = JsonSerializer.Serialize(info);
            return ww;
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

        [HttpPost]
        [Route("[controller]/Reserve")]
        public async Task<string> ReserveItem(ReservationItem data)
        {
            await _itemService.AddReservationItem(data);
            return "Success";
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
