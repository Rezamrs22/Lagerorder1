using System;
using System.Collections.Generic;

namespace Lagerorder1.Shared.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class CreateOrderDetailDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class OrderConfirmationDto
    {
        public int OrderId { get; set; }         

        public string OrderNumber { get; set; } = "";
        public string BarcodeSvg { get; set; } = "";
    }

}

