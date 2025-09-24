using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Lagerorder1.Shared.Models;

namespace Lagerorder1.Shared.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockStatus { get; set; }
        public int NumberSold { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int SizeId { get; set; }
        public Size? Size { get; set; }
    }
}