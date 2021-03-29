using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class AddDeleteItemDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Val { get; set; }
    }
}