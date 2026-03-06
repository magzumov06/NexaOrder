using Domain.DTO.Product;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ProductService(DataContext context) : IProductService
{
    public async Task<string> CreateProductAsync(CreateProductDto productDto)
    {
        try
        {
            var newProduct = new Product()
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
            };

            await context.Products.AddAsync(newProduct);
            await context.SaveChangesAsync();
            return "Product created";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while creating product: {ex.Message}");
        }
    }

    public async Task<bool> UpdateProductAsync(UpdateProductDto productDto)
    {
        try
        {
            var product = await context.Products.FirstOrDefaultAsync(x=>x.Id == productDto.Id);
            if (product == null)
            {
                return false;
            }
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Error while updating product: {e.Message}");
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return false;
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Error while deleting product: {e.Message}");
        }
    }

    public async Task<GetProductDto?> GetProductAsync(int id)
    {
        try
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return null;
            }

            var dto = new GetProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
            };
            return dto;
        }
        catch (Exception e)
        {
            throw new Exception($"Error while getting product: {e.Message}");
        }
    }

    public async Task<List<GetProductDto>> GetAllProductAsync()
    {  
        try
        {
            var products = await context.Products
                .Select(p => new GetProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity
                })
                .ToListAsync();

            return products;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while getting all products: {ex.Message}");
        }
    }
}