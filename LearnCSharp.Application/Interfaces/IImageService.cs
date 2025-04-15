using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);

        void DeleteImage(string thumbnailPath);
    }
}