using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class ChangePasswordView
    {
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [StringLength(50, ErrorMessage = "8 ถึง 50 ตัวอักษร", MinimumLength = 8)]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public string ConfirmNewPassword { get; set; }
    }
}