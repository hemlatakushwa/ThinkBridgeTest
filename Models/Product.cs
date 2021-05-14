using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace ThinkBridge.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal mrp { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public string description { get; set; }
        public int[] images { get; set; }
    }
}