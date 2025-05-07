using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class ProductSizeRequest
    {
        [Required(ErrorMessage = "Size is required")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

    }

    public class ToppingForCreate
    {
        [Required(ErrorMessage = "Topping ID is required")]
        public int ToppingId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

    }

    public class CreateProductParentRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }

        public string Description { get; set; }
        [Required(ErrorMessage = "Image is required")]

        public bool ToppingAllowed { get; set; }
        public IFormFile ParentImage { get; set; }
        public List<ProductSizeRequest> Sizes { get; set; } = new List<ProductSizeRequest>();
        public bool Status { get; set; } = true;
        public List<ToppingForCreate> Toppings { get; set; } = new List<ToppingForCreate>();

    }
}