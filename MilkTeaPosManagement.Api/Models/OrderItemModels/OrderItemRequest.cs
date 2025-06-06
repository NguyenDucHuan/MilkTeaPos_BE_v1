﻿using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.OrderItemModels
{
    public class OrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        public List<int?>? ToppingIds { get; set; }
    }
}
