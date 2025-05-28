using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerApi.Interfaces;
using ExpenseTrackerApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Get api/user
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // Get api/user/{id}
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }
    }
}