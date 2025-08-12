using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CategoryDTO model)
        {
            try
            {
                var data = new Category()
                {
                    Name = model.Name
                };
                await _unitOfWork.Category.CreateAsync(data);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Category.GetByfilterAsync(a => a.Id == id);
            try
            {
                await _unitOfWork.Category.RemoveAsync(category);
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<CategoryListItemDTO>> GetAllCategoryAsync()
        {
            var category = await _unitOfWork.Category.GetAllAsync();
            var data = category.Select(a => new CategoryListItemDTO()
            {
                Id = a.Id,
                Name = a.Name,
            });
            return data;
        }

        public async Task<CategoryListItemDTO> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Category.GetByfilterAsync(a => a.Id == id);
            var data = new CategoryListItemDTO()
            {
                Id = category.Id,
                Name = category.Name,
            };
            return data;
        }

        public async Task Update(int id, CategoryDTO model)
        {
            var category = await _unitOfWork.Category.GetByfilterAsync(a => a.Id == id);
            if (category == null)
            {
                throw new InvalidOperationException($"Categorty with ID {id} not found.");
            };
            category.Name = model.Name;
            try
            {
                _unitOfWork.Category.Update(category);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}