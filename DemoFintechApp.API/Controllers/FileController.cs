using DemoFintechApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoFintechApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("uploadImage")]
        public async Task<IActionResult> UploadFile(IFormFile _IFormFile)
        {
            var result = await _fileService.UploadImage(_IFormFile);
            return Ok(result);
        }

        [HttpGet]
        [Route("downloadImage")]
        public async Task<IActionResult> DownloadFile(string FileName)
        {
            var result = await _fileService.DownloadImage(FileName);
            return File(result.Item1, result.Item2, result.Item2);
        }
    }
}
