using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class AuthenticationDTO
    {
        public UserDTO UserInfo {get; set;}
        public string Token {get; set;}
    }
}