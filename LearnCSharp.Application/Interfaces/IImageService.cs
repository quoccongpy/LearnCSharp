using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task<List<string>> UploadMultipleImageAsync(IList<IFormFile> files);

        void DeleteImage(string thumbnailPath);

        void DeleteMultipleImage(IList<string> imagePath);
    }
}