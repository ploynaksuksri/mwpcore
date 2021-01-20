using System;
using Volo.Abp.Application.Services;

namespace Mwp.Communications
{
    public interface IPhoneAppService : ICrudAppService<PhoneDto, Guid, GetPhonesInput, PhoneCreateDto, PhoneUpdateDto>
    {
    }
}