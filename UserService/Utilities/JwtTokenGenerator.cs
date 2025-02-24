using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models;

namespace UserService.Utilities
{
	public class JwtTokenGenerator
	{
		private readonly IConfiguration _config;
		public JwtTokenGenerator(IConfiguration config)
		{
			_config = config;
		}

		public string GenerateJwtToken(UserModel user)
		{
			var jwtSettings = _config.GetSection("Jwt");
			var keyString = jwtSettings["Key"];
			if (string.IsNullOrEmpty(keyString))
			{
				throw new InvalidOperationException("Jwt key is missing");
			}
			var key = Encoding.UTF8.GetBytes(keyString);

			var securityKey = new SymmetricSecurityKey(key);
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{ 
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("userId", user.Id.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
