using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Communications
{
    [Authorize(MwpPermissions.Addresses.Default)]
    public class AddressAppService : MwpAppService, IAddressAppService
    {
        readonly IRepository<Address, Guid> _addressRepository;

        public AddressAppService(IRepository<Address, Guid> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public virtual async Task<PagedResultDto<AddressDto>> GetListAsync(GetAddressesInput input)
        {
            var totalCount = await _addressRepository.GetCountAsync();
            var items = await _addressRepository.GetListAsync();

            return new PagedResultDto<AddressDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Address>, List<AddressDto>>(items)
            };
        }

        public virtual async Task<AddressDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Address, AddressDto>(await _addressRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Addresses.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _addressRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Addresses.Create)]
        public virtual async Task<AddressDto> CreateAsync(AddressCreateDto input)
        {
            var address = ObjectMapper.Map<AddressCreateDto, Address>(input);
            address.TenantId = CurrentTenant.Id;
            address = await _addressRepository.InsertAsync(address, autoSave: true);
            return ObjectMapper.Map<Address, AddressDto>(address);
        }

        [Authorize(MwpPermissions.Addresses.Edit)]
        public virtual async Task<AddressDto> UpdateAsync(Guid id, AddressUpdateDto input)
        {
            var address = await _addressRepository.GetAsync(id);
            ObjectMapper.Map(input, address);
            address = await _addressRepository.UpdateAsync(address);
            return ObjectMapper.Map<Address, AddressDto>(address);
        }
    }
}