using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Utilities;

namespace UserService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private static readonly List<UserModel> Users = new()
		{
			new UserModel { Id = 1, Username = "user1", Password = "password1", Email = "user1@gmail.com" },
			new UserModel { Id = 2, Username = "user2", Password = "password2", Email = "user2@gmail.com" }
		};

		private readonly JwtTokenGenerator _jwtTokenGenerator;

		public UsersController(JwtTokenGenerator jwtTokenGenerator)
		{
			_jwtTokenGenerator=jwtTokenGenerator;
		}

		[Authorize]
		[HttpGet("{id}")]
		public IActionResult GetByd(int id)
		{
			var user = Users.FirstOrDefault(u => u.Id == id);
			if (user == null) return NotFound();
			return Ok(user);
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginModel request)
		{
			var user = Users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
			if (user == null) return Unauthorized();

			var token = _jwtTokenGenerator.GenerateJwtToken(user);
			return Ok(new { Token = "fake-jwt-token", UserId = user.Id});
		}
	}
}
