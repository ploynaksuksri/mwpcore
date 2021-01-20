using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Mwp.Localization;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Mwp.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class MwpController : AbpController
    {
        protected MwpController()
        {
            LocalizationResource = typeof(MwpResource);
        }

        protected IActionResult AsJson(string json)
        {
            return Content(json, "application/json");
        }

        protected IActionResult AsJson<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return Content(json, "application/json");
        }

        protected IActionResult BuildResponseFromBusinessError(BusinessException err)
        {
            var status = (int)HttpStatusCode.InternalServerError;
            var statusDict = new Dictionary<string, int>
            {
                { HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest },
                { HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound },
                { HttpStatusCode.Unauthorized.ToString(), (int)HttpStatusCode.Unauthorized },
                { HttpStatusCode.Forbidden.ToString(), (int)HttpStatusCode.Forbidden }
            };
            if (statusDict.ContainsKey(err.Code))
            {
                status = statusDict[err.Code];
            }

            return StatusCode(status, err.Message);
        }
    }
}