using FluentAssertions;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Crust;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace LearnCSharp.Tests.Application.Services
{
    public class CrustServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRedisCacheService> _mockRedisCache;
        private readonly Mock<ICrustRepository> _mockCrustRepo;
        private readonly CrustService _sut;

        public CrustServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRedisCache = new Mock<IRedisCacheService>();
            _mockCrustRepo = new Mock<ICrustRepository>();
            _mockUnitOfWork.Setup(a => a.Crust).Returns(_mockCrustRepo.Object);
            _sut = new CrustService(_mockUnitOfWork.Object, _mockRedisCache.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_CallsRepoAndSavesDb()
        {
            var model = new CrustDTO()
            {
                Name = "Mỏng"
            };
            _mockUnitOfWork.Setup(a => a.CompleteAsync()).ReturnsAsync(1);

            await _sut.CreateAsync(model);

            _mockCrustRepo.Verify(a => a.CreateAsync(It.Is<Crust>(a => a.Name == "Mỏng")), Times.Once());
            _mockUnitOfWork.Verify(a => a.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_RemovesCrustAndSavesDb()
        {
            var crust = new Crust()
            {
                Id = 3,
                Name = "Mỏng"
            };
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(crust);
            _mockUnitOfWork.Setup(a => a.CompleteAsync()).ReturnsAsync(1);

            await _sut.DeleteAsync(3);

            _mockCrustRepo.Verify(a => a.RemoveAsync(crust), Times.Once());
            _mockUnitOfWork.Verify(a => a.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Crust)null);

            await _sut.Invoking(s => s.DeleteAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");

            _mockCrustRepo.Verify(r => r.RemoveAsync(It.IsAny<Crust>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllCrustAsync_WithData_ReturnsAllCrusts()
        {
            var fakeCrusts = new List<Crust>
            {
                new() { Id = 1, Name = "Đế dày" },
                new() { Id = 2, Name = "Đế mỏng" },
                new() { Id = 3, Name = "Đế nhân mật ong" },
            };

            _mockCrustRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(fakeCrusts);

            var result = await _sut.GetAllCrustAsync();
            result.Should().HaveCount(3);
            var items = result.ToList();

            items[0].Id.Should().Be(1);
            items[0].Name.Should().Be("Đế dày");

            items[1].Id.Should().Be(2);
            items[1].Name.Should().Be("Đế mỏng");

            items[2].Id.Should().Be(3);
            items[2].Name.Should().Be("Đế nhân mật ong");
        }

        [Fact]
        public async Task GetAllCrustAsync_WhenEmpty_ReturnsEmptyList()
        {
            _mockCrustRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(new List<Crust>());
            var result = await _sut.GetAllCrustAsync();
            result.Should().BeEmpty();
        }

        public async Task GetByIdAsync_WithValidId_ReturnsCrustDTO()
        {
            var fakeCrust = new Crust { Id = 1, Name = "Đế dày" };
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(fakeCrust);
            var result = await _sut.GetByIdAsync(1);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Đế dày");
            result.Should().BeOfType<CrustListItemDTO>();
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Crust)null);
            await _sut.Invoking(s => s.GetByIdAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");
            _mockCrustRepo.Verify(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Update_WithValidId_UpdatesNameAndSavesDb()
        {
            var existing = new Crust { Id = 3, Name = "Đế nhân mật ong" };
            var updateModel = new CrustDTO { Name = "Đế nhân phomai" };

            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            await _sut.Update(3, updateModel);
            existing.Name.Should().Be("Đế nhân phomai");
            _mockCrustRepo.Verify(r => r.Update(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_WithInvalidId_ThrowsInvalidOperationException()
        {
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Crust)null);
            await _sut.Invoking(s => s.Update(999, new CrustDTO { Name = "X" })).Should().ThrowAsync<InvalidOperationException>().WithMessage("*999*");
            _mockCrustRepo.Verify(r => r.Update(It.IsAny<Crust>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldOnlyUpdateName_NotAffectOtherProperties()
        {
            var existing = new Crust
            {
                Id = 3,
                Name = "Old Name",
                ProductVariants = new List<ProductVariant>()
            };
            _mockCrustRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Crust, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(3, new CrustDTO { Name = "New Name" });
            existing.Id.Should().Be(3);
            existing.Name.Should().Be("New Name");
            existing.ProductVariants.Should().NotBeNull();
        }
    }
}