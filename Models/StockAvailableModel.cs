using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace doggo.Models
{
    public class ReserveAvailable
    {
        public List<ReserveItem> reserveList { get; set; }
    }

    public class ReserveItem
    {
        public int itemId { get; set; }
        public String name { get; set; }
        public String location { get; set; }
        public ReserveItemAmount amount { get; set; }
    }
    
    public class ReserveItemAmount
    {
        public int t0910 { get; set; }
        public int t1011 { get; set; }
        public int t1112 { get; set; }
        public int t1213 { get; set; }
        public int t1314 { get; set; }
        public int t1415 { get; set; }
        public int t1516 { get; set; }
        public int t1617 { get; set; }
        public int t1718 { get; set; }
        public int t1819 { get; set; }
        
    }
}