using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        [StringLength(50, ErrorMessage = "ชื่อ-สกุล สูงสุด 50 ตัวอักษร")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "อีเมลไม่ถูกต้อง")]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "รหัสผ่าน ตั้งแต่ 8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [StringLength(50, ErrorMessage = "รหัสผ่าน ตั้งแต่ 8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string ConfirmPassword { get; set; }
    }
}