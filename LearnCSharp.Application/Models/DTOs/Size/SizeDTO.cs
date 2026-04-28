using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Application.Models.DTOs.Size
{
    public class SizeDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}