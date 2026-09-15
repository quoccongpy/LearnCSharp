using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models.DTOs.ProductVariant;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(ProductVariantCreateDTO model)
        {
            var data = new ProductVariant()
            {
                ProductId = model.ProductId,
                CrustId = model.CrustId,
                SizeId = model.SizeId,
                Price = model.Price,
            };
            await _unitOfWork.ProductVariant.CreateAsync(data);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByfilterAsync(a => a.Id == id);
            if (productVariant == null)
            {
                throw new KeyNotFoundException($"Product Variant with ID {id} not found.");
            }
            await _unitOfWork.ProductVariant.RemoveAsync(productVariant);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ProductVariantListItemDTO> GetByIdAsync(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdIncludeAsync(a => a.Id == id);
            if (productVariant == null)
            {
                throw new KeyNotFoundException($"ProductVariant with ID {id} not found.");
            }
            var data = new ProductVariantListItemDTO()
            {
                Id = productVariant.Id,
                ProductId = productVariant.ProductId,
                CrustName = productVariant.Crust.Name,
                ProductName = productVariant.Product.Name,
                SizeName = productVariant.Size.Name,
                Price = productVariant.Price,
            };
            return data;
        }

        public async Task<List<ProductVariantListItemDTO>> GetByProductIdAsync(int productId)
        {
            var variants = await _unitOfWork.ProductVariant.GetAllAsync(pv => pv.ProductId == productId,
                                                                        tracked: false,
                                                                        pv => pv.Size,
                                                                        pv => pv.Crust
                                                                       );
            return variants.Select(pv => new ProductVariantListItemDTO
            {
                Id = pv.Id,
                ProductId = pv.ProductId,
                SizeName = pv.Size.Name,
                CrustName = pv.Crust.Name,
                Price = pv.Price,
                SizeId = pv.SizeId,
                CrustId = pv.CrustId,
            }).OrderByDescending(a => a.Id).ToList();
        }

        public async Task<List<ProductSimpleDTO>> GetProductsWithVariantAsync()
        {
            var products = await _unitOfWork.ProductVariant.GetProductsHasVariantsAsync();
            return products.Select(p => new ProductSimpleDTO
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }

        public async Task Update(int id, ProductVariantUpdateDTO model)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByfilterAsync(a => a.Id == id);
            if (productVariant == null)
            {
                throw new InvalidOperationException($"ProductVariant with ID {id} not found.");
            };
            productVariant.SizeId = model.SizeId ?? productVariant.SizeId;
            productVariant.CrustId = model.CrustId ?? productVariant.CrustId;
            productVariant.Price = model.Price ?? productVariant.Price;
            _unitOfWork.ProductVariant.Update(productVariant);
            await _unitOfWork.CompleteAsync();
        }
    }
}