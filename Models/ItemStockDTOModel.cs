using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class ItemStockDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        public string Type { get; set; }
        public int Amount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}