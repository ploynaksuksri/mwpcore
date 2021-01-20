using System;
using Volo.Abp.Application.Services;

namespace Mwp.Communications
{
    public interface IAddressAppService : ICrudAppService<AddressDto, Guid, GetAddressesInput, AddressCreateDto, AddressUpdateDto>
    {
    }
}