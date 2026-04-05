using LearnCSharp.Application.Models;

namespace LearnCSharp.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(FileUploadModel file);

        Task<List<string>> UploadMultipleImageAsync(IList<FileUploadModel> files);

        void DeleteImage(string thumbnailPath);

        void DeleteMultipleImage(IList<string> imagePath);
    }
}