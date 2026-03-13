using Domain.DTO.User;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelegramBot.Controllers;


[ApiController]
[Route("api/user")]
public class UserController(IUserService userService,
    DataContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        var res = await userService.CreateUserAsync(createUserDto);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var res = await userService.UpdateUserAsync(updateUserDto);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        var res = await userService.DeleteUserAsync(id);
        return Ok(res);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserAsync(int id)
    {
        var res = await userService.GetUserAsync(id);
        if (res == null)
            return NotFound();
        return Ok(res);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync()
    {
        var res = await userService.GetUsersAsync();
        return Ok(res);
    }
    
    [HttpGet("phone/{phone}")]
    public async Task<IActionResult> GetUserByPhoneAsync(string phone)
    {
        var user = await userService.GetUserByPhoneAsync(phone);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
    
    
    [HttpGet("role/{telegramId}")]
    public async Task<IActionResult> GetRole(long telegramId)
    {
        var res = await userService.GetRole(telegramId);

        if (res == null)
            return NotFound();

        return Ok(new { Role = res.Role });
    }
}