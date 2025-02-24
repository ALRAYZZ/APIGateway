using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using System.Net.NetworkInformation;

namespace OrderService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class OrdersController : ControllerBase
	{
		private static readonly List<OrderModel> Orders = new();

		[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(Orders);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var result = Orders.FirstOrDefault(o => o.Id == id);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}

		[HttpPost]
		public IActionResult Create([FromBody] OrderModel order)
		{
			order.Id = Orders.Count + 1;
			order.OrderDate = DateTime.Now;
			Orders.Add(order);
			return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
		}
	}
}
