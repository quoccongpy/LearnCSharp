using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Crust;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class CrustService : ICrustService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;

        public CrustService(IUnitOfWork unitOfWork, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
        }

        public async Task CreateAsync(CrustDTO model)
        {
            var data = new Crust()
            {
                Name = model.Name
            };
            await _unitOfWork.Crust.CreateAsync(data);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var crust = await _unitOfWork.Crust.GetByfilterAsync(a => a.Id == id);
            if (crust == null)
            {
                throw new KeyNotFoundException($"Crust with ID {id} not found.");
            }
            await _unitOfWork.Crust.RemoveAsync(crust);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<CrustListItemDTO>> GetAllCrustAsync()
        {
            //var cacheKey = SD.CategoriesAll;
            //var cached = await _redisCacheService.GetAsync<List<CategoryListItemDTO>>(cacheKey);
            //if (cached != null)
            //{
            //    return cached;
            //}
            var crust = await _unitOfWork.Crust.GetAllAsync();
            var data = crust.Select(a => new CrustListItemDTO()
            {
                Id = a.Id,
                Name = a.Name,
            });
            //await _redisCacheService.SetAsyc(cacheKey, data, TimeSpan.FromHours(6));
            return data;
        }

        public async Task<CrustListItemDTO> GetByIdAsync(int id)
        {
            var crust = await _unitOfWork.Crust.GetByfilterAsync(a => a.Id == id);
            if (crust == null)
            {
                throw new KeyNotFoundException($"Crust with ID {id} not found.");
            }
            var data = new CrustListItemDTO()
            {
                Id = crust.Id,
                Name = crust.Name,
            };
            return data;
        }

        public async Task Update(int id, CrustDTO model)
        {
            var crust = await _unitOfWork.Crust.GetByfilterAsync(a => a.Id == id);
            if (crust == null)
            {
                throw new InvalidOperationException($"Crust with ID {id} not found.");
            };
            crust.Name = model.Name;
            _unitOfWork.Crust.Update(crust);
            await _unitOfWork.CompleteAsync();
            //await _redisCacheService.RemoveAsync(SD.CategoriesAll);
        }
    }
}