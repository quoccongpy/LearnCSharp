namespace LearnCSharp.Application.Models
{
    public class FileUploadModel
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}