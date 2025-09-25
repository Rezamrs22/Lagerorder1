using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Lagerorder1.Shared.Models;

namespace Lagerorder1.Shared.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        
        public List<OrderDetail> OrderDetails { get; set; }= new();
    }

}