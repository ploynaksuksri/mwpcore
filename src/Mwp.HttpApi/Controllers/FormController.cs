using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mwp.Form;
using Mwp.Form.Dtos;
using Newtonsoft.Json.Linq;

namespace Mwp.Controllers
{
    public class FormController : MwpController
    {
        private readonly IFormAppService _formService;

        public FormController(IFormAppService formService)
        {
            _formService = formService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/submission/{submissionId}")]
        [HttpGet("form/undefined/submission/{submissionId}")]
        public async Task<IActionResult> GetFormSubmissionById(
            [FromRoute] Guid? formId,
            [FromRoute] Guid submissionId)
        {
            var json = await _formService.GetSubmissionByIdAsJsonAsync(
                formId.GetValueOrDefault(),
                submissionId);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}")]
        public async Task<IActionResult> GetFormById(Guid formId)
        {
            var json = await _formService.GetFormByIdAsJsonAsync(formId);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/parents")]
        public async Task<IActionResult> GetParentFormsByFormId(Guid formId)
        {
            var result = await _formService.GetParentFormsByFormId(formId);
            return AsJson(result.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/children")]
        public async Task<IActionResult> GetChildrenFormByFormId(Guid formId)
        {
            var result = await _formService.GetChildrenFormByFormId(formId);
            return AsJson(result.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("form/{formId}/submission")]
        public async Task<IActionResult> SaveSubmissionWithFormId(
            [FromRoute] Guid formId,
            [FromBody] JToken token)
        {
            var json = await _formService.SaveSubmission(formId, token);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("form")]
        public async Task<IActionResult> SaveForm([FromBody] JToken token)
        {
            var json = await _formService.SaveForm(token);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("form/{formId}")]
        public async Task<IActionResult> DeleteForm(
            [FromRoute] Guid formId)
        {
            await _formService.DeleteForm(formId);
            return Json(new { Success = true });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("form/{formId}/submission/{submissionId}")]
        public async Task<IActionResult> DeleteSubmission(
            [FromRoute] Guid formId,
            [FromRoute] Guid submissionId)
        {
            await _formService.DeleteSubmission(formId, submissionId);
            return Json(new { Success = true });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("form/{formId}/submission/{submissionId}")]
        [HttpPut("form/undefined/submission/{submissionId}")]
        public async Task<IActionResult> UpdateSubmissionWithFormId(
            [FromRoute] Guid formId,
            [FromRoute] Guid submissionId,
            [FromBody] JToken token)
        {
            if (token[FormIoProps.ObjId] == null || new Guid(token[FormIoProps.ObjId].Value<string>()) != submissionId)
            {
                return BadRequest();
            }

            var json = await _formService.SaveSubmission(formId, token);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/list")]
        public async Task<IActionResult> ListForm(
            [FromQuery] string searchText,
            [FromQuery] long skip,
            [FromQuery] long take)
        {
            var json = await _formService.ListForm(searchText, skip, take);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form")]
        public async Task<IActionResult> GetForms(
            [FromQuery] long limit,
            [FromQuery] string select)
        {
            var columns = new[] { "_id", "title" };
            if (!string.IsNullOrEmpty(select))
            {
                columns = select.Split(",");
            }

            var rows = await _formService.GetFormsForLookUp(limit, columns);
            return AsJson(rows.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/submission")]
        public async Task<IActionResult> ListSubmission(
            [FromRoute] Guid formId,
            [FromQuery] long skip,
            [FromQuery] long take)
        {
            var json = await _formService.ListSubmission(formId, skip, take);
            return AsJson(json);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/history")]
        public async Task<IActionResult> ListFormnHistory(Guid formId)
        {
            var rows = await _formService.ListFormHistory(formId);
            if (rows == null)
            {
                return NotFound();
            }

            return AsJson(rows.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/history/{recordId}")]
        public async Task<IActionResult> GetFormHistory(
            [FromRoute] Guid formId,
            [FromRoute] string recordId)
        {
            var obj = await _formService.GetFormHistory(formId, recordId);
            if (obj == null)
            {
                return NotFound();
            }

            return AsJson(obj.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("submission/{submissionId}/history")]
        public async Task<IActionResult> ListSubmissionHistory(
            [FromRoute] Guid submissionId)
        {
            var rows = await _formService.ListSubmissionHistory(submissionId);
            if (rows == null)
            {
                return NotFound();
            }

            return AsJson(rows.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("submission/{submissionId}/history/{recordId}")]
        public async Task<IActionResult> GetSubmissionHistory(
            [FromRoute] Guid submissionId,
            [FromRoute] string recordId)
        {
            var obj = await _formService.GetSubmissionHistory(submissionId, recordId);
            if (obj == null)
            {
                return NotFound();
            }

            return AsJson(obj.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("form/user/{userId}/form-builder-config")]
        public async Task<IActionResult> SaveFormBuilderConfiguration(
            [FromRoute] Guid userId,
            [FromBody] JToken formBuilderConfig)
        {
            var config = await _formService.SaveFormBuilderConfiguration(userId, formBuilderConfig);
            return AsJson(config.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/user/{userId}/form-builder-config")]
        public async Task<IActionResult> GetFormBuilderConfiguration([FromRoute] Guid userId)
        {
            var config = await _formService.GetFormBuilderConfiguration(userId);
            if (config == null)
            {
                return Json(null);
            }

            return AsJson(config.ToString());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("form/rollover")]
        public async Task<IActionResult> RolloverForm([FromBody] FormRolloverInfoDto formRollover)
        {
            var result = await _formService.RolloverForm(formRollover);
            return AsJson(new { Success = result });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("form/{formId}/import-submission-from-excel")]
        public async Task<IActionResult> ImportSubmissionFromExcel([FromRoute] Guid formId)
        {
            var file = Request.Form.Files[0];
            var data = new byte[file.Length];
            using (var stream = file.OpenReadStream())
            {
                await stream.ReadAsync(data, 0, (int)file.Length);
            }

            var result = await _formService.ImportSubmissionFromExcel(formId, data);
            return Json(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("form/{formId}/export-to-excel")]
        public async Task<IActionResult> ExportSubmissionToExcel(
            [FromRoute] Guid formId,
            [FromQuery] bool blank = false)
        {
            var result = await _formService.ExportSubmissionToExcel(formId, blank);
            if (result == null)
            {
                return NotFound();
            }

            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("form/{formId}/restore-to/{historyId}")]
        public async Task<IActionResult> RestoreFormToVersion(
            [FromRoute] Guid formId,
            [FromRoute] Guid historyId)
        {
            await _formService.RestoreFormToVersion(formId, historyId);
            await Task.Delay(10);
            return Json(new { Success = true });
        }
    }
}