using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Lagerorder1.Shared.Models;

namespace Lagerorder1.Shared.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public List<Product> Products { get; set; } = new();
    }
}