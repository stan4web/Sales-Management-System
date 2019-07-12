using Microsoft.AspNetCore.Http;

namespace SalesApp.Models.FileHelper
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
