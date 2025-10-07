using Lagerorder1.Shared.Models;
using Lagerorder1.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Lagerorder1.Services
{
    public static class MappingService
    {
        public static IQueryable<ProductDto> MapToProductDtos(this IQueryable<Product> q)
            => q.Select(p => new ProductDto
            {
                ProductId    = p.ProductId,
                Name         = p.Name,
                Description  = p.Description,
                Price        = p.Price,
                StockStatus  = p.StockStatus,
                NumberSold   = p.NumberSold,

                CategoryId   = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,

                SizeId       = p.SizeId,
                SizeName     = p.Size != null ? p.Size.Name : null 
            });

        public static CategoryDto MapToCategoryDto(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ProductNames = category.Products?.Select(p => p.Name).ToList() ?? new List<string>()
            };
        }

        // Projektion för queries (lista och single via Where + FirstOrDefaultAsync)
        public static IQueryable<CategoryDto> ToCategoryDtos(this IQueryable<Category> source)
            => source.Select(c => new CategoryDto
            {
                CategoryId   = c.CategoryId,
                Name         = c.Name,
                ProductNames = c.Products.Select(p => p.Name).ToList()
            });

        // (valfritt) Hjälpmetod för single-get
        public static Task<CategoryDto?> ToCategoryDtoByIdAsync(this IQueryable<Category> source, int id, CancellationToken ct = default)
            => source.Where(c => c.CategoryId == id)
                     .Select(c => new CategoryDto
                     {
                         CategoryId   = c.CategoryId,
                         Name         = c.Name,
                         ProductNames = c.Products.Select(p => p.Name).ToList()
                     })
                     .FirstOrDefaultAsync(ct);

        public static SizeDto MapToSizeDto(Size size)
        {
            return new SizeDto
            {
                SizeId = size.SizeId,
                Name = size.Name,
                ProductNames = size.Products?.Select(p => p.Name).ToList() ?? new List<string>()
            };
        }

 
        public static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name ?? "", // kräver att Product har en Name
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

        }
        public static OrderDetailDto MapToOrderDetailDto(OrderDetail orderDetail)
        {
            return new OrderDetailDto
            {
                OrderDetailId = orderDetail.OrderDetailId,
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.Product?.Name ?? "", // kräver att Product har en Name
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice
            };
        }

    }
}
