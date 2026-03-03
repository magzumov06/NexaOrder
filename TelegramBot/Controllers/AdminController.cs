using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly IOrderService1 _orders;

    public AdminController(IOrderService1 orders)
    {
        _orders = orders;
    }

    private bool IsAdmin()
        => Request.Headers["Admin-Key"] == "SuperSecret";

    [HttpGet("orders")]
    public async Task<IActionResult> All()
    {
        if (!IsAdmin()) return Unauthorized();
        return Ok(await _orders.GetAll());
    }

    [HttpPut("orders/{id}")]
    public async Task<IActionResult> Update(
        int id, OrderStatus status)
    {
        if (!IsAdmin()) return Unauthorized();
        await _orders.UpdateStatus(id, status);
        return Ok();
    }

    [HttpDelete("orders/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return Unauthorized();
        await _orders.Delete(id);
        return Ok();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        if (!IsAdmin()) return Unauthorized();
        var result = await _orders.Stats();
        return Ok(result);
    }
}