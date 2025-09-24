using Lagerorder1.Shared.Models;
using Lagerorder1.Shared.DTOs;

namespace Lagerorder1.Services
{
    public static class MappingService
    {
        public static ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                CategoryName = product.Category?.Name,
                SizeName = product.Size?.Name,
                Price = product.Price
            };
        }
        public static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList() ?? new List<OrderDetailDto>()
            };
        }

        public static CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ProductNames = category.Products?.Select(p => p.Name).ToList() ?? new List<string>()
            };
        }

        public static SizeDto MapToSizeDto(Size size)
        {
            return new SizeDto
            {
                SizeId = size.SizeId,
                Name = size.Name,
                ProductNames = size.Products?.Select(p => p.Name).ToList() ?? new List<string>()
            };
        }
    }
}