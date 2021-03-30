using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class HistoryView
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemLocation { get; set; }
        public int Timeslot { get; set; }
        public DateTime ReserveDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}