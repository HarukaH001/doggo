using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class ItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}