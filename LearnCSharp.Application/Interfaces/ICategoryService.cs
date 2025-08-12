using LearnCSharp.Application.Models.DTOs.Category;

namespace LearnCSharp.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListItemDTO>> GetAllCategoryAsync();
        Task<CategoryListItemDTO> GetByIdAsync(int id);

        Task CreateAsync(CategoryDTO model);
        Task Update(int id,CategoryDTO model);
        Task DeleteAsync(int id);
    }
}