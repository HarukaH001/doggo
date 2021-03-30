using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class TimeTable
    {
        public int UserId { get; set; }
        public string Name { get; set;}
        public List<int> Timeslot { get; set; }
    }
}