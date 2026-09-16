using FluentAssertions;
using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Notification;
using LearnCSharp.Application.Services;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace LearnCSharp.Tests.Application.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<INotificationRepository> _mockNotificationRepo;
        private readonly NotificationService _sut;
        private readonly Guid _loggedInUserId = Guid.NewGuid();
        public NotificationServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockNotificationRepo = new Mock<INotificationRepository>();
            _mockUnitOfWork.Setup(u => u.Notification).Returns(_mockNotificationRepo.Object);
            _mockCurrentUser.SetupGet(u => u.UserId).Returns(_loggedInUserId);
            _sut = new NotificationService(_mockUnitOfWork.Object, _mockCurrentUser.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesEntityWithAllFields()
        {
            var targetUserId = Guid.NewGuid(); 
            var model = new NotificationDTO
            {
                Title = "Đơn hàng đã được xác nhận",
                Message = "Đơn hàng #123 của bạn đang được xử lý",
                Type = "order",
                OrderId = 123,
            };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(targetUserId, model);
            _mockNotificationRepo.Verify(r => r.CreateAsync(
                It.Is<Notification>(n =>
                    n.UserId == targetUserId &&
                    n.Title == "Đơn hàng đã được xác nhận" &&
                    n.Message == "Đơn hàng #123 của bạn đang được xử lý" &&
                    n.Type == "order" &&
                    n.OrderId == 123 &&
                    n.IsRead == false)),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task CreateAsync_UsesUserIdFromParameter_NotFromCurrentUserService()
        {
            var differentUserId = Guid.NewGuid();
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(differentUserId, new NotificationDTO
            {
                Title = "Test",
                Message = "Test msg",
                Type = "system"
            });
            _mockNotificationRepo.Verify(r => r.CreateAsync(It.Is<Notification>(n => n.UserId == differentUserId)),Times.Once);
            _mockCurrentUser.VerifyGet(u => u.UserId, Times.Never);
        }
        [Fact]
        public async Task CreateAsync_WithNullOrderId_CreatesNotificationWithoutOrderId()
        {
            var userId = Guid.NewGuid();
            var model = new NotificationDTO
            {
                Title = "Chào mừng!",
                Message = "Tài khoản đã được tạo thành công",
                Type = "system",
                OrderId = null  
            };
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.CreateAsync(userId, model);
            _mockNotificationRepo.Verify(r => r.CreateAsync(It.Is<Notification>(n => n.OrderId == null)),Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotificationExists_DeletesAndSaves()
        {
            var entity = new Notification
            {
                Id = 5,
                UserId = _loggedInUserId,
                Title = "Test"
            };
            _mockNotificationRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Notification, bool>>>(),It.IsAny<bool>())).ReturnsAsync(entity);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.DeleteAsync(5);
            _mockNotificationRepo.Verify(r => r.RemoveAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_WhenNotificationNotFound_DoesNothingSilently()
        {
            _mockNotificationRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Notification, bool>>>(), It.IsAny<bool>())).ReturnsAsync((Notification)null);
            await _sut.Invoking(s => s.DeleteAsync(999)).Should().NotThrowAsync();
            _mockNotificationRepo.Verify(r => r.RemoveAsync(It.IsAny<Notification>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Fact]
        public async Task DeleteAsync_UsesCurrentUserIdToFilterNotification()
        {
            _mockNotificationRepo.Setup(r => r.GetByfilterAsync(It.IsAny<Expression<Func<Notification, bool>>>(),It.IsAny<bool>())).ReturnsAsync((Notification)null);
            await _sut.DeleteAsync(1);
            _mockCurrentUser.VerifyGet(u => u.UserId, Times.Once);
        }

        [Fact]
        public async Task GetByUserAsync_ReturnsMappedDTOsAndTotalCount()
        {
            var fakeEntities = new List<Notification>
            {
                new() { Id = 1, UserId = _loggedInUserId, Title = "Thông báo 1",
                        Message = "Nội dung 1", Type = "order",  OrderId = 10,
                        IsRead = false, CreatedAt = new DateTime(2026, 9, 1) },
                new() { Id = 2, UserId = _loggedInUserId, Title = "Thông báo 2",
                        Message = "Nội dung 2", Type = "system", OrderId = null,
                        IsRead = true,  CreatedAt = new DateTime(2026, 9, 2) },
            };
            _mockNotificationRepo.Setup(r => r.GetPagedAsync(It.IsAny<Expression<Func<Notification, bool>>>(),It.IsAny<int>(),It.IsAny<int>(),It.IsAny<bool>(),It.IsAny<Expression<Func<Notification, object>>[]>())).ReturnsAsync((fakeEntities, 2));
            var (items, total) = await _sut.GetByUserAsync(pageIndex: 1, pageSize: 10);
            items.Should().HaveCount(2);
            total.Should().Be(2);
            var first = items.First();
            first.Id.Should().Be(1);
            first.Title.Should().Be("Thông báo 1");
            first.Message.Should().Be("Nội dung 1");
            first.Type.Should().Be("order");
            first.OrderId.Should().Be(10);
            first.IsRead.Should().BeFalse();
            var second = items[1];
            second.IsRead.Should().BeTrue();
            second.OrderId.Should().BeNull();
        }

        [Fact]
        public async Task GetByUserAsync_PassesCorrectSkipAndTake()
        {
            _mockNotificationRepo.Setup(r => r.GetPagedAsync(It.IsAny<Expression<Func<Notification, bool>>>(),It.IsAny<int>(),It.IsAny<int>(),It.IsAny<bool>(),It.IsAny<Expression<Func<Notification, object>>[]>())).ReturnsAsync((new List<Notification>(), 0));
            await _sut.GetByUserAsync(pageIndex: 3, pageSize: 5);
            _mockNotificationRepo.Verify(r => r.GetPagedAsync(It.IsAny<Expression<Func<Notification, bool>>>(),10,5, It.IsAny<bool>(),It.IsAny<Expression<Func<Notification, object>>[]>()),Times.Once);
        }
        [Fact]
        public async Task GetByUserAsync_WhenEmpty_ReturnsEmptyListAndZeroCount()
        {
            _mockNotificationRepo.Setup(r => r.GetPagedAsync(It.IsAny<Expression<Func<Notification, bool>>>(),It.IsAny<int>(),It.IsAny<int>(),It.IsAny<bool>(),It.IsAny<Expression<Func<Notification, object>>[]>())).ReturnsAsync((new List<Notification>(), 0));
            var (items, total) = await _sut.GetByUserAsync(1, 10);
            items.Should().BeEmpty();
            total.Should().Be(0);
        }
        [Fact]
        public async Task GetUnreadCountAsync_ReturnsCountFromRepo()
        {
            _mockNotificationRepo.Setup(r => r.GetUnreadCountAsync(_loggedInUserId)).ReturnsAsync(7);
            var count = await _sut.GetUnreadCountAsync();
            count.Should().Be(7);
            _mockCurrentUser.VerifyGet(u => u.UserId, Times.Once);
        }
        [Fact]
        public async Task GetUnreadCountAsync_WhenNoUnread_ReturnsZero()
        {
            _mockNotificationRepo.Setup(r => r.GetUnreadCountAsync(_loggedInUserId)).ReturnsAsync(0);
            var count = await _sut.GetUnreadCountAsync();
            count.Should().Be(0);
        }
        [Fact]
        public async Task MarkAsReadAsync_CallsRepoWithCorrectIdAndUserId()
        {
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.MarkAsReadAsync(id: 42);
            _mockNotificationRepo.Verify(r => r.MarkAsReadAsync(42, _loggedInUserId),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task MarkAsReadAsync_PassesCurrentUserIdToRepo()
        {
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.MarkAsReadAsync(1);
            _mockCurrentUser.VerifyGet(u => u.UserId, Times.Once);
            _mockNotificationRepo.Verify( r => r.MarkAsReadAsync(It.IsAny<int>(), _loggedInUserId),Times.Once);
        }
        [Fact]
        public async Task MarkAllAsReadAsync_CallsRepoWithCurrentUserIdAndSaves()
        {
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.MarkAllAsReadAsync();
            _mockNotificationRepo.Verify(r => r.MarkAllAsReadAsync(_loggedInUserId),Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task MarkAllAsReadAsync_CallsCompleteAsyncExactlyOnce()
        {
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            await _sut.MarkAllAsReadAsync();
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}