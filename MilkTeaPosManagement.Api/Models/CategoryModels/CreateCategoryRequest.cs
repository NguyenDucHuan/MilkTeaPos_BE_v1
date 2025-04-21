using System.ComponentModel.DataAnnotations;

namespace MilkTeaPosManagement.Api.Models.CategoryModels
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }
        public bool Status { get; set; } = true;
    }
}
