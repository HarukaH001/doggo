using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class ReservationItem
    {
        public int itemId { get; set; }
        public int userId { get; set; }
        public List<int> timeslot { get; set; }
        public DateTime reserveDate { get; set; }
    }
}