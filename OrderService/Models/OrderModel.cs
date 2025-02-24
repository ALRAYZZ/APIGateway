﻿namespace OrderService.Models
{
	public class OrderModel
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
	}
}
