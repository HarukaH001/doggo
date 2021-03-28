using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class UserStatusDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}