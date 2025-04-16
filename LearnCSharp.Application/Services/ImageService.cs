using LearnCSharp.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LearnCSharp.Application.Services
{
    // largeFile :https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/5.x/LargeFilesSample/Controllers/FileUploadController.cs?fbclid=IwY2xjawJrAzlleHRuA2FlbQIxMAABHuzqpb8QoJBzikX5F1kH13GcvqxvwU9aCs0DQEpycsGDYskm25yjXo8io69w_aem_NlvhpSmhwOqztYvmmHZpZA
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void DeleteImage(string thumbnailPath)
        {
            if (string.IsNullOrEmpty(thumbnailPath))
            {
                return;
            }
            string filePath = Path.Combine(_environment.WebRootPath, thumbnailPath.TrimStart('/'));
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteMultipleImage(IList<string> imagePath)
        {
            if (imagePath == null || imagePath.Any())
            {
                return;
            }
            foreach (var path in imagePath)
            {
                DeleteImage(path);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".gif"
            };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, .png, .gif are allowed.");
            }
            if (file.Length > 1 * 1024 * 1024)
            {
                throw new ArgumentException("File size should not exceed 1MB");
            }
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "product");
            Directory.CreateDirectory(uploadFolder);
            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/images/product/{fileName}";
        }

        public async Task<List<string>> UploadMultipleImageAsync(IList<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return new List<string>();
            }
            var uploadPath = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    string path = await UploadImageAsync(file);
                    uploadPath.Add(path);
                }
                catch (Exception ex)
                {
                    DeleteMultipleImage(uploadPath);
                    throw new Exception(ex.Message);
                }
            }
            return uploadPath;
        }
    }
}