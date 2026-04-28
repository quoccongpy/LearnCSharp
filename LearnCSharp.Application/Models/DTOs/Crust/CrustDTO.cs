using System.ComponentModel.DataAnnotations;

namespace LearnCSharp.Application.Models.DTOs.Crust
{
    public class CrustDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}