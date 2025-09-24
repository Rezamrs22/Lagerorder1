using System;
using System.Collections.Generic;

namespace Lagerorder1.Shared.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
