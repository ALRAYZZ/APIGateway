using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<JwtTokenGenerator>();

var jwtSetting = builder.Configuration.GetSection("Jwt");
var keyString = jwtSetting["Key"];
if (string.IsNullOrEmpty(keyString))
{
	throw new InvalidOperationException("Jwt key is missing");
}
var key = Encoding.UTF8.GetBytes(keyString);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSetting["Issuer"],
			ValidAudience = jwtSetting["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};
	});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
