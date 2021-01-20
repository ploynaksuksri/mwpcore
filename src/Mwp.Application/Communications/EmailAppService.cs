using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Communications
{
    [Authorize(MwpPermissions.Emails.Default)]
    public class EmailAppService : MwpAppService, IEmailAppService
    {
        readonly IRepository<Email, Guid> _emailRepository;

        public EmailAppService(IRepository<Email, Guid> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public virtual async Task<PagedResultDto<EmailDto>> GetListAsync(GetEmailsInput input)
        {
            var totalCount = await _emailRepository.GetCountAsync();
            var items = await _emailRepository.GetListAsync();

            return new PagedResultDto<EmailDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Email>, List<EmailDto>>(items)
            };
        }

        public virtual async Task<EmailDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Email, EmailDto>(await _emailRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Emails.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _emailRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Emails.Create)]
        public virtual async Task<EmailDto> CreateAsync(EmailCreateDto input)
        {
            var email = ObjectMapper.Map<EmailCreateDto, Email>(input);
            email.TenantId = CurrentTenant.Id;
            email = await _emailRepository.InsertAsync(email, autoSave: true);
            return ObjectMapper.Map<Email, EmailDto>(email);
        }

        [Authorize(MwpPermissions.Emails.Edit)]
        public virtual async Task<EmailDto> UpdateAsync(Guid id, EmailUpdateDto input)
        {
            var email = await _emailRepository.GetAsync(id);
            ObjectMapper.Map(input, email);
            email = await _emailRepository.UpdateAsync(email);
            return ObjectMapper.Map<Email, EmailDto>(email);
        }
    }
}