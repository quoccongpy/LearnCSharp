using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Size;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(SizeDTO model)
        {
            var data = new Size()
            {
                Name = model.Name
            };
            await _unitOfWork.Size.CreateAsync(data);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var size = await _unitOfWork.Size.GetByfilterAsync(a => a.Id == id);
            if (size == null)
            {
                throw new KeyNotFoundException($"Size with ID {id} not found.");
            }
            await _unitOfWork.Size.RemoveAsync(size);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<SizeListItemDTO>> GetAllSizeAsync()
        {
            var size = await _unitOfWork.Size.GetAllAsync();
            var data = size.Select(a => new SizeListItemDTO()
            {
                Id = a.Id,
                Name = a.Name,
            });
            return data;
        }

        public async Task<SizeListItemDTO> GetByIdAsync(int id)
        {
            var size = await _unitOfWork.Size.GetByfilterAsync(a => a.Id == id);
            if (size == null)
            {
                throw new KeyNotFoundException($"Size with ID {id} not found.");
            }
            var data = new SizeListItemDTO()
            {
                Id = size.Id,
                Name = size.Name,
            };
            return data;
        }

        public async Task Update(int id, SizeDTO model)
        {
            var size = await _unitOfWork.Size.GetByfilterAsync(a => a.Id == id);
            if (size == null)
            {
                throw new InvalidOperationException($"Size with ID {id} not found.");
            };
            size.Name = model.Name;
            _unitOfWork.Size.Update(size);
            await _unitOfWork.CompleteAsync();
        }
    }
}