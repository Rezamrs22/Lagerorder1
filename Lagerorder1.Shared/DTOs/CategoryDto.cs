using System;
using System.Collections.Generic;

namespace Lagerorder1.Shared.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public List<string> ProductNames { get; set; } = new();
    }
}
