using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class ItemInfoView
    {
        public StockSummaryDTO StockSummary { get; set; }
        public IEnumerable<StockRecordDTO> StockRecord { get; set; }
    }
}