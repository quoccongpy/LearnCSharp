using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Product;

namespace LearnCSharp.Application.Interfaces
{
    public interface IProductService
    {
        Task CreateAsync(ProductCreateDTO model);

        Task Update(int id, ProductUpdateDTO model);

        Task<PagedResult<ProductDTO>> GetAllProductPagingAsync(string? keyword, int? categoryId, int pageIndex = 1, int pageSize = 10);

        Task<ProductDTO> GetByIdAsync(int id);

        Task DeleteAsync(int id);
    }
}