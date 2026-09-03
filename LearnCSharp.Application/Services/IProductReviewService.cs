using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Review;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using System.Linq.Expressions;

namespace LearnCSharp.Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public ProductReviewService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<bool> CanUserReviewAsync(int productId)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                return false;
            }
            var hasPurchased = await _unitOfWork.ProductReview.HasUserPurchasedAndDeliveredAsync(userId, productId);
            if (!hasPurchased)
            {
                return false;
            }
            var alreadyReviewed = await _unitOfWork.ProductReview.HasUserAlreadyReviewedAsync(userId, productId);
            return !alreadyReviewed;
        }

        public async Task CreateAsync(ProductReviewCreateDTO model)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("You must be logged in to submit a review.");
            }
            if (model.Rating < 1 || model.Rating > 5)
            {
                throw new ApplicationException("Rating must be between 1 and 5.");
            }
            var hasPurchased = await _unitOfWork.ProductReview.HasUserPurchasedAndDeliveredAsync(userId, model.ProductId);
            if (!hasPurchased)
            {
                throw new ApplicationException("You can only review products you have purchased and received.");
            }
            var alreadyReviewed = await _unitOfWork.ProductReview.HasUserAlreadyReviewedAsync(userId, model.ProductId);
            if (alreadyReviewed)
            {
                throw new ApplicationException("You have already reviewed this product. Please edit your existing review.");
            }
            var review = new ProductReview
            {
                ProductId = model.ProductId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                IsHidden = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.ProductReview.CreateAsync(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int reviewId)
        {
            var userId = _currentUserService.UserId;
            var isAdmin = await _userService.IsUserInRoleAsync(userId, SD.RoleAdmin);
            var review = await _unitOfWork.ProductReview.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            if (!isAdmin && review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this review.");
            }
            await _unitOfWork.ProductReview.RemoveAsync(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PagedResult<ProductReviewDTO>> GetAllForAdminAsync(int pageIndex, int pageSize, string keyword)
        {
            var userId = _currentUserService.UserId;
            var isAdmin = await _userService.IsUserInRoleAsync(userId, SD.RoleAdmin);
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Only admins are authorized to view all reviews.");
            }
            Expression<Func<ProductReview, bool>> filter;

            if (string.IsNullOrEmpty(keyword))
            {
                filter = r => true;
            }
            else
            {
                filter = r => r.Comment.Contains(keyword);
            }
            var skip = (pageIndex - 1) * pageSize;
            var (reviews, totalCount) = await _unitOfWork.ProductReview.GetPagedAsync(filter, skip, pageSize);
            var dtoList = await MapToDTOsAsync(reviews);
            return new PagedResult<ProductReviewDTO>
            {
                Results = dtoList,
                CurrentPage = pageIndex,
                RowCount = totalCount,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ProductReviewDTO>> GetByProductIdAsync(int productId, int pageIndex, int pageSize)
        {
            var userId = _currentUserService.UserId;
            var skip = (pageIndex - 1) * pageSize;
            var (reviews, totalCount) = await _unitOfWork.ProductReview.GetByProductIdPagedAsync(productId, skip, pageSize, includeHidden: false);
            var hiddenOwn = userId != Guid.Empty? await _unitOfWork.ProductReview.GetAllAsync( r => r.ProductId == productId && r.UserId == userId && r.IsHidden) : Enumerable.Empty<ProductReview>();
            var combined = reviews.Concat(hiddenOwn).GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
            var dtoList = await MapToDTOsAsync(combined);
            return new PagedResult<ProductReviewDTO>
            {
                Results = dtoList,
                CurrentPage = pageIndex,
                RowCount = totalCount,
                PageSize = pageSize
            };
        }

        public async Task HideAsync(int reviewId, bool isHidden)
        {
            var userId = _currentUserService.UserId;
            var isAdmin = await _userService.IsUserInRoleAsync(userId, SD.RoleAdmin);
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Only admins are authorized to hide or unhide reviews.");
            }
            var review = await _unitOfWork.ProductReview.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            review.IsHidden = isHidden;
            _unitOfWork.ProductReview.Update(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(int reviewId, ProductReviewUpdateDTO model)
        {
            var userId = _currentUserService.UserId;
            var review = await _unitOfWork.ProductReview.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this review.");
            }
            if (model.Rating < 1 || model.Rating > 5)
            {
                throw new ApplicationException("Rating must be between 1 and 5.");
            }
            review.Rating = model.Rating;
            review.Comment = model.Comment;
            _unitOfWork.ProductReview.Update(review);
            await _unitOfWork.CompleteAsync();
        }
        private async Task<List<ProductReviewDTO>> MapToDTOsAsync(IEnumerable<ProductReview> reviews)
        {
            var result = new List<ProductReviewDTO>();
            foreach (var r in reviews)
            {
                var user = await _userService.GetUserByIdAsync(r.UserId);
                result.Add(new ProductReviewDTO
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = user?.UserName ?? "Ẩn danh",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsHidden = r.IsHidden,
                    CreatedAt = r.CreatedAt,
                });
            }
            return result;
        }
    }
}