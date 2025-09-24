using System;
using System.Collections.Generic;

namespace Lagerorder1.Shared.DTOs
{
    public class SizeDto
    {
        public int SizeId { get; set; }
        public string Name { get; set; }

        public List<string> ProductNames { get; set; } = new();
    }
}