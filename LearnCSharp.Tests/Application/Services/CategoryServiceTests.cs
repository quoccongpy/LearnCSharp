using FluentAssertions;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Category;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Linq.Expressions;
//https://github-wiki-see.page/m/devlooped/moq/wiki/Quickstart
namespace LearnCSharp.Tests.Application.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRedisCacheService> _mockRedisCache;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly CategoryService _sut;

        public CategoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRedisCache = new Mock<IRedisCacheService>();
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockUnitOfWork.Setup(a => a.Category).Returns(_mockCategoryRepo.Object);
            _sut = new CategoryService(_mockUnitOfWork.Object, _mockRedisCache.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectCategory()
        {
            var fakeCategory = new Category { Id = 1, Name = "Pizza" };
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync(fakeCategory);
            var result = await _sut.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Pizza");
        }
        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Category)null);
            await _sut.Invoking(s => s.GetByIdAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");
        }
        [Fact]
        public async Task GetAllCategoryAsync_WhenCacheHit_NeverCallsDatabase()
        {
            var cached = new List<CategoryListItemDTO>
            {
                new() { Id = 1, Name = "Pizza" },
                new() { Id = 2, Name = "Burger" }
            };
            _mockRedisCache.Setup(r => r.GetAsync<List<CategoryListItemDTO>>(It.IsAny<string>())).ReturnsAsync(cached);
            var result = await _sut.GetAllCategoryAsync();
            result.Should().HaveCount(2);
            _mockCategoryRepo.Verify( r => r.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>()),Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_CreatesAndClearsCache()
        {
            var model = new CategoryDTO { Name = "Sushi" };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockCategoryRepo.Verify(r => r.CreateAsync(It.Is<Category>(c => c.Name == "Sushi")),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
            _mockRedisCache.Verify(r => r.RemoveAsync(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesAndClearsCache()
        {
            var category = new Category { Id = 5, Name = "Test" };
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync(category);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.DeleteAsync(5);
            _mockCategoryRepo.Verify(r => r.RemoveAsync(category), Times.Once);
            _mockRedisCache.Verify(r => r.RemoveAsync(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException_AndDoesNotDelete()
        {
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Category)null);
            await _sut.Invoking(s => s.DeleteAsync(999)) .Should().ThrowAsync<KeyNotFoundException>();
            _mockCategoryRepo.Verify(r => r.RemoveAsync(It.IsAny<Category>()), Times.Never);
        }
        [Fact]
        public async Task Update_WithValidId_UpdatesName()
        {
            var existing = new Category { Id = 3, Name = "Pizza" };
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(3, new CategoryDTO { Name = "Burger" });
            existing.Name.Should().Be("Burger");
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task Update_WithInvalidId_ThrowsInvalidOperationException()
        {
            _mockCategoryRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Category)null);
            await _sut.Invoking(s => s.Update(999, new CategoryDTO { Name = "X" })).Should().ThrowAsync<InvalidOperationException>();
        }
    }
}