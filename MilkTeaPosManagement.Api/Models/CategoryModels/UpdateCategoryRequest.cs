﻿namespace MilkTeaPosManagement.Api.Models.CategoryModels
{
    public class UpdateCategoryRequest
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
