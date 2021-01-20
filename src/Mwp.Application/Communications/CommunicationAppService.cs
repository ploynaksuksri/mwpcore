using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Mwp.Permissions;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Communications
{
    [Authorize(MwpPermissions.Communications.Default)]
    public class CommunicationAppService : MwpAppService, ICommunicationAppService
    {
        readonly IRepository<Communication, Guid> _communicationRepository;

        public CommunicationAppService(IRepository<Communication, Guid> communicationRepository)
        {
            _communicationRepository = communicationRepository;
        }

        public virtual async Task<PagedResultDto<CommunicationDto>> GetListAsync(GetCommunicationsInput input)
        {
            var totalCount = await _communicationRepository.GetCountAsync();
            var items = await _communicationRepository.GetListAsync();

            return new PagedResultDto<CommunicationDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Communication>, List<CommunicationDto>>(items)
            };
        }

        public virtual async Task<CommunicationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Communication, CommunicationDto>(await _communicationRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Communications.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _communicationRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Communications.Create)]
        public virtual async Task<CommunicationDto> CreateAsync(CommunicationCreateDto input)
        {
            var communication = ObjectMapper.Map<CommunicationCreateDto, Communication>(input);
            communication.TenantId = CurrentTenant.Id;
            communication = await _communicationRepository.InsertAsync(communication, autoSave: true);
            return ObjectMapper.Map<Communication, CommunicationDto>(communication);
        }

        [Authorize(MwpPermissions.Communications.Edit)]
        public virtual async Task<CommunicationDto> UpdateAsync(Guid id, CommunicationUpdateDto input)
        {
            var communication = await _communicationRepository.GetAsync(id);
            ObjectMapper.Map(input, communication);
            communication = await _communicationRepository.UpdateAsync(communication);
            return ObjectMapper.Map<Communication, CommunicationDto>(communication);
        }
    }
}