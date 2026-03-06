using Domain.DTO.Product;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Controllers;


[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var res = await productService.CreateProductAsync(dto);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] UpdateProductDto dto)
    {
        var res = await productService.UpdateProductAsync(dto);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await productService.DeleteProductAsync(id);
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var res = await productService.GetProductAsync(id);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var res = await productService.GetAllProductAsync();
        return Ok(res);
    }
}