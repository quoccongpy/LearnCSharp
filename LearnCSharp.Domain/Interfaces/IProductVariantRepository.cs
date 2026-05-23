using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IProductVariantRepository : IRepository<ProductVariant>
    {
        void Update(ProductVariant productVariant);

        Task<List<Product>> GetProductsHasVariantsAsync();
    }
}