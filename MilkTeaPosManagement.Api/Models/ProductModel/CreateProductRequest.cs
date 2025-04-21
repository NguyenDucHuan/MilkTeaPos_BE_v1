using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }

        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Prize is required")]
        public decimal Prize { get; set; }

        [Required(ErrorMessage = "Product type is required")]
        public string ProductType { get; set; }

        public int? ParentId { get; set; }

        public string SizeId { get; set; }

        public bool Status { get; set; } = true;
    }
}
