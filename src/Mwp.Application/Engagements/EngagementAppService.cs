using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Mwp.Permissions;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Engagements
{
    [Authorize(MwpPermissions.Engagements.Default)]
    public class EngagementAppService : MwpAppService, IEngagementAppService
    {
        readonly IRepository<Engagement, Guid> _engagementRepository;

        public EngagementAppService(IRepository<Engagement, Guid> engagementRepository)
        {
            _engagementRepository = engagementRepository;
        }

        public virtual async Task<PagedResultDto<EngagementDto>> GetListAsync(GetEngagementsInput input)
        {
            var totalCount = await _engagementRepository.GetCountAsync();
            var items = await _engagementRepository.GetListAsync();

            return new PagedResultDto<EngagementDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Engagement>, List<EngagementDto>>(items)
            };
        }

        public virtual async Task<EngagementDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Engagement, EngagementDto>(await _engagementRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Engagements.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _engagementRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Engagements.Create)]
        public virtual async Task<EngagementDto> CreateAsync(EngagementCreateDto input)
        {
            var engagement = ObjectMapper.Map<EngagementCreateDto, Engagement>(input);
            engagement.TenantId = CurrentTenant.Id;
            engagement = await _engagementRepository.InsertAsync(engagement, autoSave: true);
            return ObjectMapper.Map<Engagement, EngagementDto>(engagement);
        }

        [Authorize(MwpPermissions.Engagements.Edit)]
        public virtual async Task<EngagementDto> UpdateAsync(Guid id, EngagementUpdateDto input)
        {
            var engagement = await _engagementRepository.GetAsync(id);
            ObjectMapper.Map(input, engagement);
            engagement = await _engagementRepository.UpdateAsync(engagement);
            return ObjectMapper.Map<Engagement, EngagementDto>(engagement);
        }
    }
}