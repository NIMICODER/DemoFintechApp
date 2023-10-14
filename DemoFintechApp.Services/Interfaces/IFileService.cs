using Microsoft.AspNetCore.Http;

namespace DemoFintechApp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImage(IFormFile _IFormFile);
        Task<(byte[], string, string)> DownloadImage(string FileName);

    }
}
