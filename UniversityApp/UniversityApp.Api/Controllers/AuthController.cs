using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityApp.Core.Entites;
using UniversityApp.Service.Dtos.UserDtos;
using UniversityApp.Service.Interfaces;

namespace UniversityApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[HttpGet("users")]
        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser user = new AppUser
        //    {
        //        FullName = "Hikmet Abbsov",
        //        UserName = "hiko123",
        //    };

        //    await _userManager.CreateAsync(user, "Hikmet123");
        //    return Ok(user.Id);
        //}


        [HttpPost("login")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            return Ok(_authService.Login(loginDto));
        }

        [HttpGet]
        public ActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var username = User.Identity.Name;
            return Ok(username);
        }

        [Authorize]
        [HttpGet("profile1")]
        public ActionResult Profile1()
        {
            return Ok();
        }

    }
}
