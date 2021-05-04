using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using doggo.Data;
using doggo.Models;
using doggo.Services;

namespace doggo.Controllers{
    // [Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
    
    [AllowAnonymous]
    [Route("api/[action]")]
    [ApiController]
    public class DefaultController : Controller
    {
        private IUserService _userService;
        private IItemService _itemService;

        public DefaultController(IUserService userService, IItemService itemService)
        {
            _userService = userService;
            _itemService = itemService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<Backpass> Authenticate([FromBody] LoginView credential)
        {
            var backpass = _userService.Authenticate(credential);
            return backpass;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Users()
        {
            var users =  _userService.GetAll();
            return Ok(users);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult StockSummary()
        {
            var users =  _itemService.StockSummary();
            return Ok(users);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TimeTable(int id)
        {
            var data =  await _itemService.GetReservationByItemId(id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> ReserveAvailable(int? year, int? month, int? day)
        {
            var data = new ReserveAvailable();
            if(year == null || month == null || day == null) data =  await _itemService.GetReserveAvailables(DateTime.Today);
            else  data = await _itemService.GetReserveAvailables(new DateTime(year.Value, month.Value, day.Value));
            return Ok(data);
        }

    }
}