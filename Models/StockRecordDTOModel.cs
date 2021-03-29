using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class StockRecordDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public int Snapshot { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}