using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Communications
{
    [Authorize(MwpPermissions.Phones.Default)]
    public class PhoneAppService : MwpAppService, IPhoneAppService
    {
        readonly IRepository<Phone, Guid> _phoneRepository;

        public PhoneAppService(IRepository<Phone, Guid> phoneRepository)
        {
            _phoneRepository = phoneRepository;
        }

        public virtual async Task<PagedResultDto<PhoneDto>> GetListAsync(GetPhonesInput input)
        {
            var totalCount = await _phoneRepository.GetCountAsync();
            var items = await _phoneRepository.GetListAsync();

            return new PagedResultDto<PhoneDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Phone>, List<PhoneDto>>(items)
            };
        }

        public virtual async Task<PhoneDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Phone, PhoneDto>(await _phoneRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Phones.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _phoneRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Phones.Create)]
        public virtual async Task<PhoneDto> CreateAsync(PhoneCreateDto input)
        {
            var phone = ObjectMapper.Map<PhoneCreateDto, Phone>(input);
            phone.TenantId = CurrentTenant.Id;
            phone = await _phoneRepository.InsertAsync(phone, autoSave: true);
            return ObjectMapper.Map<Phone, PhoneDto>(phone);
        }

        [Authorize(MwpPermissions.Phones.Edit)]
        public virtual async Task<PhoneDto> UpdateAsync(Guid id, PhoneUpdateDto input)
        {
            var phone = await _phoneRepository.GetAsync(id);
            ObjectMapper.Map(input, phone);
            phone = await _phoneRepository.UpdateAsync(phone);
            return ObjectMapper.Map<Phone, PhoneDto>(phone);
        }
    }
}