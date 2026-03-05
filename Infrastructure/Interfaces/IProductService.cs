using Domain.DTO.Product;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task CreateProductAsync(CreateProductDto productDto);
    Task<bool> UpdateProductAsync(UpdateProductDto productDto);
    Task<bool> DeleteProductAsync(int id);
    Task<GetProductDto?> GetProductAsync(int id);
    Task<List<GetProductDto>> GetAllProductAsync();
}