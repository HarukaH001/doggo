using System;
using System.ComponentModel.DataAnnotations;

namespace doggo.Models
{
    public class StockSummaryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public int Current { get; set; }
        public int Increment { get; set; }
        public int Decrement { get; set; }
    }
}