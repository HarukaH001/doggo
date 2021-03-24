using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public decimal Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password")]
        public decimal ConfirmPassword { get; set; }
    }
}