using System;
using Volo.Abp.Application.Services;

namespace Mwp.Communications
{
    public interface IEmailAppService : ICrudAppService<EmailDto, Guid, GetEmailsInput, EmailCreateDto, EmailUpdateDto>
    {
    }
}