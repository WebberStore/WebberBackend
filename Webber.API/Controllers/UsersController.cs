using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBaseEx
{
    /// <summary>
    /// Retrieves the currently authenticated user's details.
    /// </summary>
    /// <returns>
    /// Returns an Ok response with the user details if found.
    /// </returns>
    [AuthorizeMiddleware(["User", "Seller", "Admin"])]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userDto = await userService.GetUserByIdAsync(CurrentUserId);
        return Ok(userDto); // Return Ok response with user details
    }

    /// <summary>
    /// Updates the currently authenticated user's details.
    /// </summary>
    /// <param name="userDto">The updated user information.</param>
    /// <returns>
    /// Returns NoContent if the update is successful.
    /// Returns Forbid if the user is not authorized to update the information.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UserDto userDto)
    {
        // Check if the user is allowed to update the information
        if (userDto.Id != CurrentUserId && CurrentUserRole != "Admin")
            return Forbid(); // Return Forbid if not authorized
        
        await userService.UpdateUserAsync(CurrentUserId, userDto);
        return NoContent(); // Return NoContent if the update is successful
    }

    /// <summary>
    /// Retrieves a user's details by their user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>
    /// Returns an Ok response with the user details if found.
    /// Returns NotFound if the user does not exist.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var userDto = await userService.GetUserByIdAsync(userId);
        if (userDto == null)
            return NotFound(); // Return NotFound if the user does not exist

        return Ok(userDto); // Return Ok response with user details
    }
    
    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <returns>
    /// Returns an Ok response with the list of all users.
    /// </returns>
    [HttpGet]
    [AuthorizeMiddleware(["Admin"])]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users); // Return Ok response with the list of users
    }
}
