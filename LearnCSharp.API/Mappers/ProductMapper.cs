using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Application.Models;
using LearnCSharp.API.Models;

namespace LearnCSharp.API.Mappers
{
    public static class ProductMapper
    {
        public static ProductCreateDTO ToDTO(ProductCreateRequest request)
        {
            return new ProductCreateDTO
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Thumbnail = ToFileUpload(request.Thumbnail),
                Images = request.Images?.Select(ToFileUpload).ToList(),
            };
        }
        public static ProductUpdateDTO ToDTO(ProductUpdateRequest request)
        {
            return new ProductUpdateDTO
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                CategoryId = request.CategoryId,
                ListRetainIdsImage = request.ListRetainIdsImage,
                Thumbnail = ToFileUpload(request.Thumbnail),
                Images = request.Images?.Select(ToFileUpload).ToList(),
            };
        }
        private static FileUploadModel? ToFileUpload(IFormFile? file)
        {
            if (file == null) return null;
            return new FileUploadModel
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
            };
        }
    }
}
