using Domain.DTO.Order;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await orderService.CreateOrderAsync(dto);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateOrderDto dto)
    {
        var result = await orderService.UpdateOrderAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await orderService.DeleteOrderAsync(id);

        if (!result)
            return NotFound("Order not found");

        return Ok("Deleted");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var order = await orderService.GetOrderAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await orderService.GetOrderAsync();
        return Ok(orders);
    }
}