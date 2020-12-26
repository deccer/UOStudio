using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOStudio.Web.Contracts;
using UOStudio.Web.Models;
using UOStudio.Web.Services;

namespace UOStudio.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserModel createUserModel)
        {
            var userId = await _userService.CreateUserAsync(
                createUserModel.UserName,
                createUserModel.Password,
                createUserModel.DisplayName,
                createUserModel.Permissions);
            return Ok(userId);
        }

        [HttpDelete]
        public async Task Delete(Guid userId)
        {
            await _userService.DeleteUserAsync(userId);
        }
    }
}
