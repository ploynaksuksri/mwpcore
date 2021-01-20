using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mwp.File;

namespace Mwp.Controllers
{
    public class FileController : MwpController
    {
        private readonly IFileAppService _fileAppService;

        public FileController(IFileAppService fileAppService)
        {
            _fileAppService = fileAppService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var data = new byte[file.Length];
            await using (var stream = file.OpenReadStream())
            {
                await stream.ReadAsync(data, 0, (int)file.Length);
            }

            var uploadResult = await _fileAppService.UploadFile(
                data,
                file.ContentType,
                file.FileName,
                file.Length);
            return Json(uploadResult);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("file/{fileId}/info")]
        public async Task<IActionResult> GetFileInfo([FromRoute] string fileId)
        {
            var fileObj = await _fileAppService.GetFileInfo(fileId);
            if (fileObj == null)
            {
                return NotFound();
            }

            return Json(fileObj);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var fileObj = await _fileAppService.DownloadFile(fileId);
            if (fileObj == null)
            {
                return NotFound();
            }

            HttpContext.Response.Headers.Add("file-name", fileObj.FileName);

            return File(fileObj.Data, fileObj.ContentType, fileObj.FileName);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("file/{fileId}")]
        public async Task<IActionResult> DeleteFile([FromRoute] string fileId)
        {
            await _fileAppService.DeleteFile(fileId);
            return Ok();
        }
    }
}