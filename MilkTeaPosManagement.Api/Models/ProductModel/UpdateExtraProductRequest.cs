using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class UpdateExtraProductRequest
    {
        [Required]
        public int ProductId { get; set; }

        [StringLength(100)]
        public string? ProductName { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        //public IFormFile? Image { get; set; }

        public bool? ToppingAllowed { get; set; }
        public decimal? Prize { get; set; }

        public bool? Status { get; set; }
    }
}
