using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class ReservationRecordDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int ExternalId { get; set; }
        public int Timeslot { get; set; }
        public DateTime ReserveDate { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}