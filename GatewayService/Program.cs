using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
	configuration
		.ReadFrom.Configuration(context.Configuration)
		.ReadFrom.Services(services)
		.Enrich.FromLogContext()
		.WriteTo.Console()
		.WriteTo.File(
			path: "logs/apigateway-.log", // Log to a file with date and time in the name
			rollingInterval: RollingInterval.Day, // New log file every day
			retainedFileCountLimit: 7 // Keep logs for 7 days
			);
				
});


// JWT authenticationQ
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"];
if (string.IsNullOrEmpty(keyString))
{
	throw new InvalidOperationException("Jwt key is missing");
}
var key = Encoding.UTF8.GetBytes(keyString);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => 
	{ 
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings["issuer"],
			ValidAudience = jwtSettings["audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};
	});


builder.Services.AddOcelot(builder.Configuration);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Middleware
app.UseSerilogRequestLogging(); // Log HTTP requests


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware to set rate limit client ID from JWT
app.Use(async (context, next) =>
{
	if (!context.User.Identity.IsAuthenticated)
	{
		var sub = context.User.FindFirst("sub")?.Value;
		if (!string.IsNullOrEmpty(sub))
		{
			context.Request.Headers["Client-Id"] = sub; // Use JWT sub as client ID
		}
	}
	await next.Invoke();
});

app.UseOcelot().Wait();
app.MapControllers();

app.Run();
