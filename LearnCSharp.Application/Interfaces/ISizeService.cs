using LearnCSharp.Application.Models.DTOs.Size;

namespace LearnCSharp.Application.Interfaces
{
    public interface ISizeService
    {
        Task<IEnumerable<SizeListItemDTO>> GetAllSizeAsync();

        Task<SizeListItemDTO> GetByIdAsync(int id);

        Task CreateAsync(SizeDTO model);

        Task Update(int id, SizeDTO model);

        Task DeleteAsync(int id);
    }
}