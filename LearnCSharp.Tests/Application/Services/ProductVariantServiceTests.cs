using FluentAssertions;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models.DTOs.ProductVariant;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LearnCSharp.Tests.Application.Services
{
    public class ProductVariantServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductVariantRepository> _mockVariantRepo;
        private readonly ProductVariantService _sut;

        public ProductVariantServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVariantRepo = new Mock<IProductVariantRepository>();
            _mockUnitOfWork.Setup(u => u.ProductVariant).Returns(_mockVariantRepo.Object);
            _sut = new ProductVariantService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_CreatesVariantWithAllFields()
        {
            var model = new ProductVariantCreateDTO
            {
                ProductId = 10,
                SizeId = 2,
                CrustId = 3,
                Price = 199000
            };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockVariantRepo.Verify(r => r.CreateAsync(It.Is<ProductVariant>(pv => pv.ProductId == 10 &&
                                                                                   pv.SizeId == 2 &&
                                                                                   pv.CrustId == 3 &&
                                                                                   pv.Price == 199000)),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task CreateAsync_WithZeroPrice_StillCreatesVariant()
        {
            var model = new ProductVariantCreateDTO
            {
                ProductId = 1,
                SizeId = 1,
                CrustId = 1,
                Price = 0
            };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockVariantRepo.Verify( r => r.CreateAsync(It.Is<ProductVariant>(pv => pv.Price == 0)),Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_RemovesAndSaves()
        {
            var variant = new ProductVariant { Id = 5, ProductId = 1, Price = 100000 };
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync(variant);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.DeleteAsync(5);
            _mockVariantRepo.Verify(r => r.RemoveAsync(variant), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync((ProductVariant)null);
            await _sut.Invoking(s => s.DeleteAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");
            _mockVariantRepo.Verify(r => r.RemoveAsync(It.IsAny<ProductVariant>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsMappedDTO()
        {
            var fakeVariant = new ProductVariant
            {
                Id = 1,
                ProductId = 10,
                Price = 150000,
                Product = new Product { Id = 10, Name = "Pizza Margherita" },
                Size = new Size { Id = 2, Name = "Large" },
                Crust = new Crust { Id = 3, Name = "Thin Crust" },
            };
            _mockVariantRepo.Setup(r => r.GetByIdIncludeAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>(),It.IsAny<Expression<Func<ProductVariant, object>>[]>())).ReturnsAsync(fakeVariant);
            var result = await _sut.GetByIdAsync(1);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ProductId.Should().Be(10);
            result.ProductName.Should().Be("Pizza Margherita");  
            result.SizeName.Should().Be("Large");              
            result.CrustName.Should().Be("Thin Crust");        
            result.Price.Should().Be(150000);
            result.Should().BeOfType<ProductVariantListItemDTO>();
        }
        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockVariantRepo.Setup(r => r.GetByIdIncludeAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>(),It.IsAny<Expression<Func<ProductVariant, object>>[]>())).ReturnsAsync((ProductVariant)null);
            await _sut.Invoking(s => s.GetByIdAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");
        }

        public async Task GetByProductIdAsync_WithVariants_ReturnsSortedDescendingById()
        {
            var fakeVariants = new List<ProductVariant>
            {
                new() { Id = 5,  ProductId = 10, Price = 100000, Size = new Size { Name = "Small" },  Crust = new Crust { Name = "Thin" } },
                new() { Id = 1,  ProductId = 10, Price = 120000,Size = new Size { Name = "Medium" }, Crust = new Crust { Name = "Thick" } },
                new() { Id = 10, ProductId = 10, Price = 150000,Size = new Size { Name = "Large" },  Crust = new Crust { Name = "Stuffed" } },
            };
            _mockVariantRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>(),It.IsAny<Expression<Func<ProductVariant, object>>[]>())).ReturnsAsync(fakeVariants);
            var result = await _sut.GetByProductIdAsync(10);
            result.Should().HaveCount(3);
            result[0].Id.Should().Be(10);   
            result[1].Id.Should().Be(5);
            result[2].Id.Should().Be(1);    
        }
        [Fact]
        public async Task GetByProductIdAsync_MapsNavigationPropertiesCorrectly()
        {
            var fakeVariants = new List<ProductVariant>
            {
                new() { Id = 1, ProductId = 5, Price = 200000,SizeId = 2, CrustId = 3,Size  = new Size  { Id = 2, Name = "Large" },Crust = new Crust { Id = 3, Name = "Cheese Crust" } }
            };
            _mockVariantRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>(),It.IsAny<Expression<Func<ProductVariant, object>>[]>())).ReturnsAsync(fakeVariants);
            var result = await _sut.GetByProductIdAsync(5);
            result.Should().HaveCount(1);
            var item = result.First();
            item.Id.Should().Be(1);
            item.ProductId.Should().Be(5);
            item.SizeId.Should().Be(2);
            item.CrustId.Should().Be(3);
            item.SizeName.Should().Be("Large");
            item.CrustName.Should().Be("Cheese Crust");
            item.Price.Should().Be(200000m);
        }
        [Fact]
        public async Task GetByProductIdAsync_WhenNoVariants_ReturnsEmptyList()
        {
            _mockVariantRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>(),It.IsAny<Expression<Func<ProductVariant, object>>[]>())).ReturnsAsync(new List<ProductVariant>());
            var result = await _sut.GetByProductIdAsync(999);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            result.Should().BeOfType<List<ProductVariantListItemDTO>>();
        }
        [Fact]
        public async Task GetProductsWithVariantAsync_ReturnsMappedProductSimpleDTOs()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = 1, Name = "Pizza Margherita" },
                new() { Id = 2, Name = "Pizza Hawaii" },
                new() { Id = 3, Name = "Burger Classic" },
            };
            _mockVariantRepo.Setup(r => r.GetProductsHasVariantsAsync()).ReturnsAsync(fakeProducts);
            var result = await _sut.GetProductsWithVariantAsync();
            result.Should().HaveCount(3);
            result.Should().AllBeOfType<ProductSimpleDTO>();
            result.Select(p => p.Id).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            result.Select(p => p.Name).Should().Contain("Pizza Hawaii");
        }
        [Fact]
        public async Task GetProductsWithVariantAsync_WhenEmpty_ReturnsEmptyList()
        {
            _mockVariantRepo.Setup(r => r.GetProductsHasVariantsAsync()).ReturnsAsync(new List<Product>());
            var result = await _sut.GetProductsWithVariantAsync();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_WithAllFields_UpdatesAllFields()
        {
            var existing = new ProductVariant
            {
                Id = 1,
                SizeId = 1,
                CrustId = 1,
                Price = 100000m
            };
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var updateModel = new ProductVariantUpdateDTO
            {
                SizeId = 3,
                CrustId = 4,
                Price = 250000
            };
            await _sut.Update(1, updateModel);
            existing.SizeId.Should().Be(3);
            existing.CrustId.Should().Be(4);
            existing.Price.Should().Be(250000);
            _mockVariantRepo.Verify(r => r.Update(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_WithNullSizeId_KeepsOriginalSizeId()
        {
            var existing = new ProductVariant
            {
                Id = 1,
                SizeId = 5,
                CrustId = 2,
                Price = 100000
            };
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var data = new ProductVariantUpdateDTO()
            {
                SizeId = null,       
                CrustId = 10,        
                Price = 200000
            };
            await _sut.Update(1, data);
       
            existing.SizeId.Should().Be(5);      
            existing.CrustId.Should().Be(10);    
            existing.Price.Should().Be(200000); 
        }
        [Fact]
        public async Task Update_WithNullCrustId_KeepsOriginalCrustId()
        {
            var existing = new ProductVariant
            {
                Id = 1,
                SizeId = 2,
                CrustId = 7,
                Price = 100000
            };
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var data = new ProductVariantUpdateDTO
            {
                SizeId = 5,
                CrustId = null,
                Price = null
            };
            await _sut.Update(1, data);
            existing.SizeId.Should().Be(5);       
            existing.CrustId.Should().Be(7);     
            existing.Price.Should().Be(100000); 
        }
        [Fact]
        public async Task Update_WithAllNullFields_KeepsAllOriginalValues()
        {
            var existing = new ProductVariant
            {
                Id = 1,
                SizeId = 2,
                CrustId = 3,
                Price = 150000
            };
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var data = new ProductVariantUpdateDTO
            {
                SizeId = null,
                CrustId = null,
                Price = null
            };
            await _sut.Update(1, data);
            existing.SizeId.Should().Be(2);
            existing.CrustId.Should().Be(3);
            existing.Price.Should().Be(150000);
            _mockVariantRepo.Verify(r => r.Update(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task Update_WithInvalidId_ThrowsInvalidOperationException()
        {
            _mockVariantRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<ProductVariant, bool>>>(),It.IsAny<bool>())).ReturnsAsync((ProductVariant)null);
            await _sut.Invoking(s => s.Update(999, new ProductVariantUpdateDTO())).Should().ThrowAsync<InvalidOperationException>().WithMessage("*999*");
            _mockVariantRepo.Verify(r => r.Update(It.IsAny<ProductVariant>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}