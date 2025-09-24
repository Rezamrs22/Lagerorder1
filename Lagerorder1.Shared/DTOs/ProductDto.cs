using System;
using System.Collections.Generic;

namespace Lagerorder1.Shared.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockStatus { get; set; }
        public int NumberSold { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }// För visning

        public int SizeId { get; set; } 
        public string? SizeName { get; set; } // För visning
    }
}
