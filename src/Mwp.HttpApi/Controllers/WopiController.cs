using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mwp.Wopi;
using Volo.Abp;
using Volo.Abp.Authorization;

namespace Mwp.Controllers
{
    public class WopiController : MwpController
    {
        private readonly IWopiRequestAppService _wopiRequestAppService;
        private readonly IWopiAppService _wopiAppService;

        public WopiController(IWopiRequestAppService wopiRequestAppService,
            IWopiAppService wopiAppService)
        {
            _wopiRequestAppService = wopiRequestAppService;
            _wopiAppService = wopiAppService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("wopi/files/{fileIdWithHistoryId}/contents")]
        public async Task<IActionResult> GetContents(string fileIdWithHistoryId)
        {
            try
            {
                return await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId, WopiRequestType.GetFile);
            }
            catch (AbpAuthorizationException)
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("wopi/files/{fileIdWithHistoryId}/contents")]
        public async Task<IActionResult> PostContents(string fileIdWithHistoryId)
        {
            try
            {
                return await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId, WopiRequestType.PutFile);
            }
            catch (AbpAuthorizationException)
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("wopi/files/{fileIdWithHistoryId}")]
        public async Task<IActionResult> Get(string fileIdWithHistoryId)
        {
            try
            {
                return await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId, WopiRequestType.CheckFileInfo);
            }
            catch (AbpAuthorizationException)
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("wopi/files/{fileIdWithHistoryId}")]
        public async Task<IActionResult> Post(string fileIdWithHistoryId)
        {
            string wopiOverride = HttpContext.Request.Headers[WopiRequestHeader.OVERRIDE];

            try
            {
                if (wopiOverride == "LOCK")
                {
                    return string.IsNullOrEmpty(HttpContext.Request.Headers[WopiRequestHeader.OLD_LOCK])
                        ? await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId, WopiRequestType.Lock)
                        : await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId,
                            WopiRequestType.UnlockAndRelock);
                }

                if (WopiConsts.OverrideRequetTypeMap.ContainsKey(wopiOverride))
                {
                    return await _wopiRequestAppService.ProcessWopiRequest(fileIdWithHistoryId,
                        WopiConsts.OverrideRequetTypeMap[wopiOverride]);
                }

                return new StatusCodeResult((int)HttpStatusCode.NotImplemented);
            }
            catch (AbpAuthorizationException)
            {
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("wopi/files/{fileId}")]
        public async Task<IActionResult> ReplaceFile([FromRoute] Guid fileId)
        {
            try
            {
                var file = Request.Form.Files[0];
                var fileContent = new byte[file.Length];
                await using (var stream = file.OpenReadStream())
                {
                    await stream.ReadAsync(fileContent, 0, (int)file.Length);
                }

                await _wopiAppService.ReplaceFile(fileId, fileContent, file.FileName);
                return Json(new { success = true });
            }
            catch (AbpAuthorizationException)
            {
                return Unauthorized();
            }
            catch (BusinessException err)
            {
                return BuildResponseFromBusinessError(err);
            }
        }
    }
}