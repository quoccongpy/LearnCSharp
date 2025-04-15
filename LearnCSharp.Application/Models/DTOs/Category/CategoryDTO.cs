using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Application.Models.DTOs.Category
{
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}