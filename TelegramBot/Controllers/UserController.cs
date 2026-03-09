using Domain.DTO.User;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Controllers;


[ApiController]
[Route("api/user")]
public class UserController(IUserService userService) : ControllerBase
{
    // [HttpPost]
    // public async Task<IActionResult> CreateUserAsync(CreateUserDto createUserDto)
    // {
    //     var res = await userService.CreateUserAsync(createUserDto);
    //     return Ok(res);
    // }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var res = await userService.UpdateUserAsync(updateUserDto);
        return Ok(res);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        var res = await userService.DeleteUserAsync(id);
        return Ok(res);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserAsync(int id)
    {
        var res = await userService.GetUserAsync(id);
        return Ok(res);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync()
    {
        var res = await userService.GetUsersAsync();
        return Ok(res);
    }
}