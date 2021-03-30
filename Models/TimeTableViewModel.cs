using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class TimeTableView
    {
        public int ItemId { get; set; }
        public string UserName { get; set; }
        public DateTime ReserveDate { get; set; }
        public IEnumerable<TimeTable> Table { get; set; }
    }
}