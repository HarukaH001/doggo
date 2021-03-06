using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class LoginView
    {
        [RegularExpression("^(.+@.+[.].+|[aA]dmin)$", ErrorMessage = "อีเมลไม่ถูกต้อง")]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string Password { get; set; }
    }
}