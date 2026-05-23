using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models.DTOs.ProductVariant;

namespace LearnCSharp.Application.Interfaces
{
    public interface IProductVariantService
    {
        Task CreateAsync(ProductVariantCreateDTO model);

        Task Update(int id, ProductVariantUpdateDTO model);

        Task<ProductVariantListItemDTO> GetByIdAsync(int id);

        Task<List<ProductSimpleDTO>> GetProductsWithVariantAsync();

        Task<List<ProductVariantListItemDTO>> GetByProductIdAsync(int productId);

        Task DeleteAsync(int id);
    }
}