using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using System.Net.NetworkInformation;

namespace ProductService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private static readonly List<ProductModel> Products = new()
		{ 
			new ProductModel { Id = 1, Name = "Keyboard", Price = 20.0m },
			new ProductModel { Id = 2, Name = "Mouse", Price = 10.0m },
		};

		[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(Products);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var product = Products.FirstOrDefault(p => p.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			return Ok(product);
		}
	}
}
