using LearnCSharp.Application.Models.DTOs.Category;

namespace LearnCSharp.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoryAsync();
        Task<CategoryDTO> GetByIdAsync(int id);

        Task CreateAsync(CategoryDTO model);
        Task Update(int id,CategoryDTO model);
        Task DeleteAsync(int id);
    }
}