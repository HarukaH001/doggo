using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class TimeTableQuery
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set;}
        public string Timeslot { get; set; }
    }
}