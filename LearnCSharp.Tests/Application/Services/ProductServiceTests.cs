using FluentAssertions;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models.DTOs.ProductImage;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace LearnCSharp.Tests.Application.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<IProductImageService> _mockProductImageService;

        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IProductImageRepository> _mockProductImageRepo;

        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockImageService = new Mock<IImageService>();
            _mockProductImageService = new Mock<IProductImageService>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockProductImageRepo = new Mock<IProductImageRepository>();
            _mockUnitOfWork.Setup(u => u.Product).Returns(_mockProductRepo.Object);
            _mockUnitOfWork.Setup(u => u.ProductImage).Returns(_mockProductImageRepo.Object);
            _sut = new ProductService(_mockUnitOfWork.Object, _mockImageService.Object, _mockProductImageService.Object);
        }

        [Fact]
        public async Task CreateAsync_WithoutThumbnailAndImages_CreatesProductAndCommits()
        {
            var model = new ProductCreateDTO
            {
                Name = "Pizza Margherita",
                Price = 150000,
                Description = "Pizza ngon",
                CategoryId = 1,
                Thumbnail = null,
                Images = null
            };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockUnitOfWork.Verify(a => a.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(a => a.CommitTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(a => a.RollbackTransactionAsync(), Times.Never);

            _mockProductRepo.Verify(r => r.CreateAsync(It.Is<Product>(p => p.Name == "Pizza Margherita" &&
                                                                          p.Price == 150000m &&
                                                                          p.CategoryId == 1 &&
                                                                          p.Thumbnail == null)), Times.Once);
            _mockImageService.Verify(s => s.UploadImageAsync(It.IsAny<FileUploadModel>()), Times.Never);
            _mockImageService.Verify(s => s.UploadMultipleImageAsync(It.IsAny<IList<FileUploadModel>>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithThumbnail_UploadsAndSavesThumbnailPath()
        {
            var fakeThumbnail = new FileUploadModel
            {
                FileName = "pizza.jpg",
                ContentType = "image/jpeg",
                FileStream = new MemoryStream()
            };
            var model = new ProductCreateDTO
            {
                Name = "Pizza",
                Price = 100000m,
                CategoryId = 1,
                Thumbnail = fakeThumbnail
            };
            _mockImageService.Setup(a => a.UploadImageAsync(fakeThumbnail)).ReturnsAsync("/uploads/pizza.jpg");
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);

            _mockProductRepo.Verify(r => r.CreateAsync(It.Is<Product>(p => p.Thumbnail == "/uploads/pizza.jpg")), Times.Once);
            _mockImageService.Verify(s => s.UploadImageAsync(fakeThumbnail), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithMultipleImages_UploadsAndCreatesProductImages()
        {
            var fakeImages = new List<FileUploadModel>
            {
                new() { FileName = "img1.jpg", ContentType = "image/jpeg", FileStream = new MemoryStream() },
                new() { FileName = "img2.jpg", ContentType = "image/jpeg", FileStream = new MemoryStream() },
            };
            var model = new ProductCreateDTO
            {
                Name = "Pizza",
                Price = 100000,
                CategoryId = 1,
                Images = fakeImages
            };

            _mockImageService.Setup(s => s.UploadMultipleImageAsync(fakeImages)).ReturnsAsync(new List<string> { "/uploads/img1.jpg", "/uploads/img2.jpg" });
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockProductImageRepo.Verify(r => r.CreateAsync(It.IsAny<ProductImage>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenUploadThumbnailFails_RollsBackTransaction()
        {
            var fakeThumbnail = new FileUploadModel
            {
                FileName = "bad.jpg",
                FileStream = new MemoryStream()
            };
            var model = new ProductCreateDTO
            {
                Name = "Pizza",
                Price = 100000,
                CategoryId = 1,
                Thumbnail = fakeThumbnail
            };
            _mockImageService.Setup(s => s.UploadImageAsync(It.IsAny<FileUploadModel>())).ThrowsAsync(new Exception("Upload failed"));
            await _sut.Invoking(a => a.CreateAsync(model)).Should().ThrowAsync<Exception>().WithMessage("Upload failed");
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesProductAndSaves()
        {
            var product = new Product { Id = 5, Name = "Old Pizza" };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(product);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.DeleteAsync(5);
            _mockProductRepo.Verify(r => r.RemoveAsync(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllProductPagingAsync_WithValidParams_ReturnsPagingResult()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = 1, Name = "Pizza A", Price = 100000, CategoryId = 1, Category = new Category { Name = "Cat A" } },
                new() { Id = 2, Name = "Pizza B", Price = 200000, CategoryId = 1, Category = new Category { Name = "Cat A" } },
            };
            _mockProductRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((fakeProducts, 2));
            var result = await _sut.GetAllProductPagingAsync(null, null, pageIndex: 1, pageSize: 10);
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(2);
            var items = result.Results.ToList();

            items[0].Id.Should().Be(1);
            items[0].Name.Should().Be("Pizza A");

            items[1].Id.Should().Be(2);
            items[1].Name.Should().Be("Pizza B");
            result.PageSize.Should().Be(10);
            result.RowCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAllProductPagingAsync_WithSearch_ReturnsMatchingProducts()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = 1, Name = "Pizza A", Price = 100000, CategoryId = 1, Category = new Category { Name = "Cat A" } },
            };
            _mockProductRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((fakeProducts, 1));
            var result = await _sut.GetAllProductPagingAsync("Pizza A", null, pageIndex: 1, pageSize: 10);
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(1);
            var item = result.Results.Single();

            item.Id.Should().Be(1);
            item.Name.Should().Be("Pizza A");

            _mockProductRepo.Verify(r => r.SearchAsync("Pizza A", null, 0, 10), Times.Once);
        }

        [Fact]
        public async Task GetAllProductPagingAsync_WithSearchCategoryId_ReturnsMatchingProducts()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = 1, Name = "Pizza A", Price = 100000, CategoryId = 1, Category = new Category { Name = "Cat A" } },
            };
            _mockProductRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((fakeProducts, 1));
            var result = await _sut.GetAllProductPagingAsync(null, 1, pageIndex: 1, pageSize: 10);
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(1);
            var item = result.Results.Single();

            item.Id.Should().Be(1);
            item.CategoryId.Should().Be(1);

            _mockProductRepo.Verify(r => r.SearchAsync(null, 1, 0, 10), Times.Once);
        }

        [Fact]
        public async Task GetAllProductPagingAsync_WithSearchAndCategoryId_ReturnsMatchingProducts()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = 1, Name = "Pizza A", Price = 100000, CategoryId = 1, Category = new Category { Name = "Cat A" } },
            };
            _mockProductRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((fakeProducts, 1));
            var result = await _sut.GetAllProductPagingAsync("Pizza A", 1, pageIndex: 1, pageSize: 10);
            result.Should().NotBeNull();
            result.Results.Should().HaveCount(1);
            var item = result.Results.Single();

            item.Id.Should().Be(1);
            item.Name.Should().Be("Pizza A");
            item.CategoryId.Should().Be(1);

            _mockProductRepo.Verify(r => r.SearchAsync("Pizza A", 1, 0, 10), Times.Once);
        }

        [Theory]
        [InlineData(0, 10, 1, 10)]
        [InlineData(-5, 10, 1, 10)]
        [InlineData(1, 0, 1, 10)]
        [InlineData(1, -3, 1, 10)]
        [InlineData(2, 5, 2, 5)]
        public async Task GetAllProductPagingAsync_NormalizesInvalidPagingParams(int inputPage, int inputSize, int expectedPage, int expectedSize)
        {
            _mockProductRepo.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((new List<Product>(), 0));
            var result = await _sut.GetAllProductPagingAsync(null, null, inputPage, inputSize);
            result.CurrentPage.Should().Be(expectedPage);
            result.PageSize.Should().Be(expectedSize);
        }

        [Fact]
        public async Task GetByCategoryAsync_WithValidCategoryId_ReturnsProducts()
        {
            var fakeProducts = new List<ProductDTO>
            {
                new(){Id = 1,Name = "Pizza A",Price = 100000},
                new(){Id = 2,Name = "Pizza B",Price = 200000},
            };

            _mockProductRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, ProductDTO>>>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(fakeProducts);
            var result = await _sut.GetByCategoryAsync(1, 10);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Pizza A");
            result[0].Price.Should().Be(100000);

            result[1].Id.Should().Be(2);
            result[1].Name.Should().Be("Pizza B");
            result[1].Price.Should().Be(200000);

            _mockProductRepo.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, ProductDTO>>>(), 10, false), Times.Once);
        }

        [Fact]
        public async Task GetByCategoryAsync_WhenNoProducts_ReturnsEmptyList()
        {
            _mockProductRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, ProductDTO>>>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new List<ProductDTO>());
            var result = await _sut.GetByCategoryAsync(999, 10);
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsProductWithImages()
        {
            var fakeProduct = new Product
            {
                Id = 1,
                Name = "Pizza",
                Price = 150000,
                Description = "Pizza ngon",
                Thumbnail = "/uploads/pizza.jpg",
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Fast Food" }
            };
            var fakeImages = new List<ProductImageDTO>
            {
                new() { Id = 10, ProductId = 1, ImageUrl = "/uploads/img1.jpg" },
                new() { Id = 11, ProductId = 1, ImageUrl = "/uploads/img2.jpg" },
            };
            _mockProductRepo.Setup(r => r.GetByIdIncludeAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(fakeProduct);
            _mockProductImageService.Setup(s => s.GetListProductImageByIdAsync(1)).ReturnsAsync(fakeImages);
            var result = await _sut.GetByIdAsync(1);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Pizza");
            result.Price.Should().Be(150000m);
            result.CategoryName.Should().Be("Fast Food");
            result.ProductImagesList.Should().HaveCount(2);
            result.ProductImagesList.First().ImageUrl.Should().Be("/uploads/img1.jpg");
        }

        [Fact]
        public async Task Update_WithNameAndPrice_UpdatesFieldsAndCommits()
        {
            var existing = new Product
            {
                Id = 1,
                Name = "Tên cũ",
                Price = 100000,
                Description = "Mô tả cũ",
                CategoryId = 1,
                Thumbnail = null
            };
            var updateModel = new ProductUpdateDTO
            {
                Name = "Tên mới",
                Price = 200000
            };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(1, updateModel);
            existing.Name.Should().Be("Tên mới");
            existing.Price.Should().Be(200000);
            existing.Description.Should().Be("Mô tả cũ");
            existing.CategoryId.Should().Be(1);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_WithNewThumbnail_DeletesOldAndUploadsNew()
        {
            var newThumbFile = new FileUploadModel
            {
                FileName = "new.jpg",
                FileStream = new MemoryStream()
            };
            var existing = new Product
            {
                Id = 1,
                Name = "Pizza",
                Price = 100000,
                Thumbnail = "/uploads/old.jpg"
            };
            var updateModel = new ProductUpdateDTO
            {
                Thumbnail = newThumbFile
            };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockImageService.Setup(s => s.UploadImageAsync(newThumbFile)).ReturnsAsync("/uploads/new.jpg");
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(1, updateModel);
            _mockImageService.Verify(s => s.DeleteImage("/uploads/old.jpg"), Times.Once);
            _mockImageService.Verify(s => s.UploadImageAsync(newThumbFile), Times.Once);
            existing.Thumbnail.Should().Be("/uploads/new.jpg");
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_WithNullName_KeepsOriginalName()
        {
            var existing = new Product { Id = 1, Name = "Tên gốc", Price = 100000 };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(1, new ProductUpdateDTO { Name = null, Description = "Mô tả mới" });
            existing.Name.Should().Be("Tên gốc");
            existing.Description.Should().Be("Mô tả mới");
        }

        [Fact]
        public async Task Update_WithPriceZeroOrNegative_KeepsOriginalPrice()
        {
            var existing = new Product { Id = 1, Name = "Pizza", Price = 150000 };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(1, new ProductUpdateDTO { Price = 0 });
            existing.Price.Should().Be(150000);
        }

        [Fact]
        public async Task Update_WithInvalidId_ThrowsAndRollsBack()
        {
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Product)null);
            await _sut.Invoking(s => s.Update(999, new ProductUpdateDTO())).Should().ThrowAsync<InvalidOperationException>().WithMessage("*999*");
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_WhenRetainSomeImages_DeletesOnlyUnretainedImages()
        {
            var existing = new Product { Id = 1, Name = "Pizza", Price = 100000 };
            var currentImages = new List<ProductImageDTO>
            {
                new() { Id = 10, ProductId = 1, ImageUrl = "/img10.jpg" },
                new() { Id = 11, ProductId = 1, ImageUrl = "/img11.jpg" },
                new() { Id = 12, ProductId = 1, ImageUrl = "/img12.jpg" },
            };
            _mockProductRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockProductImageService.Setup(s => s.GetListProductImageByIdAsync(1)).ReturnsAsync(currentImages);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            var updateModel = new ProductUpdateDTO
            {
                ListRetainIdsImage = new List<int> { 10, 11 }
            };
            await _sut.Update(1, updateModel);
            _mockImageService.Verify(s => s.DeleteImage("/img12.jpg"), Times.Once);
            _mockProductImageService.Verify(s => s.DeleteAsync(12), Times.Once);
            _mockImageService.Verify(s => s.DeleteImage("/img10.jpg"), Times.Never);
            _mockImageService.Verify(s => s.DeleteImage("/img11.jpg"), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}