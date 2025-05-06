using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.ProductModel
{
    public class UpdateComboProductRequest
    {
        [Required]
        public int ProductId { get; set; }
        [StringLength(100)]
        public string? ProductName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }

        //public IFormFile? Image { get; set; }
        public decimal? Prize { get; set; }
        public bool? Status { get; set; }
        public List<ComboItemUpdateRequest>? ComboItems { get; set; }
    }
    public class ComboItemUpdateRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int? MasterId { get; set; }
    }
}
