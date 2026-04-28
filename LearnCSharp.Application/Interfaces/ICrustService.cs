using LearnCSharp.Application.Models.DTOs.Crust;

namespace LearnCSharp.Application.Interfaces
{
    public interface ICrustService
    {
        Task<IEnumerable<CrustListItemDTO>> GetAllCrustAsync();

        Task<CrustListItemDTO> GetByIdAsync(int id);

        Task CreateAsync(CrustDTO model);

        Task Update(int id, CrustDTO model);

        Task DeleteAsync(int id);
    }
}