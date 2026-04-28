using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;

        public CategoryService(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
        }

        public async Task CreateAsync(CategoryDTO model)
        {
            var data = new Category()
            {
                Name = model.Name
            };
            await _unitOfWork.Category.CreateAsync(data);
            await _unitOfWork.CompleteAsync();
            await _redisCacheService.RemoveAsync(SD.CategoriesAll);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Category.GetByfilterAsync(a => a.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            await _unitOfWork.Category.RemoveAsync(category);
            await _unitOfWork.CompleteAsync();
            await _redisCacheService.RemoveAsync(SD.CategoriesAll);
        }

        public async Task<IEnumerable<CategoryListItemDTO>> GetAllCategoryAsync()
        {
            var cacheKey = SD.CategoriesAll;
            var cached = await _redisCacheService.GetAsync<List<CategoryListItemDTO>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }
            var category = await _unitOfWork.Category.GetAllAsync();
            var data = category.Select(a => new CategoryListItemDTO()
            {
                Id = a.Id,
                Name = a.Name,
            });
            await _redisCacheService.SetAsyc(cacheKey, data, TimeSpan.FromHours(6));
            return data;
        }

        public async Task<CategoryListItemDTO> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Category.GetByfilterAsync(a => a.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
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
            _unitOfWork.Category.Update(category);
            await _unitOfWork.CompleteAsync();
            await _redisCacheService.RemoveAsync(SD.CategoriesAll);
        }
    }
}