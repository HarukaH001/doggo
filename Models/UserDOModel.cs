using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class UserDO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string UserRole { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}