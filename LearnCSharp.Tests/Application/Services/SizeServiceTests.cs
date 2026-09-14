using FluentAssertions;
using LearnCSharp.Application.Models.DTOs.Size;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace LearnCSharp.Tests.Application.Services
{
    public class SizeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISizeRepository> _mockSizeRepo;
        private readonly SizeService _sut;

        public SizeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSizeRepo = new Mock<ISizeRepository>();
            _mockUnitOfWork.Setup(u => u.Size).Returns(_mockSizeRepo.Object);
            _sut = new SizeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_CreatesEntityWithCorrectName()
        {
            var model = new SizeDTO { Name = "Lớn" };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(model);
            _mockSizeRepo.Verify(r => r.CreateAsync(It.Is<Size>(s => s.Name == "Lớn")),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsCompleteAsync_ExactlyOnce()
        {
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(new SizeDTO { Name = "Nhỏ" });
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WithValidId_RemovesSizeAndSavesDb()
        {
            var size = new Size { Id = 5, Name = "Vừa" };
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync(size);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.DeleteAsync(5);
            _mockSizeRepo.Verify(r => r.RemoveAsync(size), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync((Size)null);
            await _sut.Invoking(s => s.DeleteAsync(99)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*99*");
            _mockSizeRepo.Verify(r => r.RemoveAsync(It.IsAny<Size>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task GetAllSizeAsync_WithData_ReturnsAllSizes()
        {
            var fakeSize= new List<Size>
            {
                new() { Id = 1, Name = "Lớn" },
                new() { Id = 2, Name = "Nhỏ" },
                new() { Id = 3, Name = "Vừa" },
            };

            _mockSizeRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Size, bool>>>(), It.IsAny<bool>())).ReturnsAsync(fakeSize);

            var result = await _sut.GetAllSizeAsync();
            result.Should().HaveCount(3);
            var items = result.ToList();

            items[0].Id.Should().Be(1);
            items[0].Name.Should().Be("Lớn");

            items[1].Id.Should().Be(2);
            items[1].Name.Should().Be("Nhỏ");

            items[2].Id.Should().Be(3);
            items[2].Name.Should().Be("Vừa");
        }
        [Fact]
        public async Task GetAllSizeAsync_WhenEmpty_ReturnsEmptyCollection()
        {
            _mockSizeRepo.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync(new List<Size>());
            var result = await _sut.GetAllSizeAsync();
            result.Should().BeEmpty();
            result.Should().NotBeNull(); 
        }
        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsSizeDTO()
        {
            // ARRANGE
            var fakeSize = new Size { Id = 1, Name = "Small" };
            _mockSizeRepo
                .Setup(r => r.GetByfilterAsync(
                    It.IsAny<Expression<Func<Size, bool>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(fakeSize);
            // ACT
            var result = await _sut.GetByIdAsync(1);
            // ASSERT
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Small");
            result.Should().BeOfType<SizeListItemDTO>();
        }
        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync((Size)null);
            await _sut.Invoking(s => s.GetByIdAsync(999)).Should().ThrowAsync<KeyNotFoundException>().WithMessage("*999*");
            _mockSizeRepo.Verify(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>()),Times.Once);
        }
        [Fact]
        public async Task Update_WithValidId_UpdatesNameAndSavesDb()
        {
            var existing = new Size { Id = 2, Name = "Old Name" };
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(2, new SizeDTO { Name = "New Name" });
            existing.Name.Should().Be("New Name");
            _mockSizeRepo.Verify(r => r.Update(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task Update_WithInvalidId_ThrowsInvalidOperationException()
        {
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync((Size)null);
        
            await _sut.Invoking(s => s.Update(999, new SizeDTO { Name = "X" })).Should().ThrowAsync<InvalidOperationException>().WithMessage("*999*");
            _mockSizeRepo.Verify(r => r.Update(It.IsAny<Size>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task Update_OnlyChangesName_IdRemainsUnchanged()
        {
            var existing = new Size { Id = 7, Name = "Old" };
            _mockSizeRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Size, bool>>>(),It.IsAny<bool>())).ReturnsAsync(existing);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.Update(7, new SizeDTO { Name = "New" });
            existing.Id.Should().Be(7);
            existing.Name.Should().Be("New");
        }
    }
}