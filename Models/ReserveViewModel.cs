using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class ReserveView
    {
        public int ItemId { get; set; }
        public List<int> Timeslot { get; set; }
        public DateTime ReserveDate { get; set; }
    }
}