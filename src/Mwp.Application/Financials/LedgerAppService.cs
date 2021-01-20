using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Financials
{
    [Authorize(MwpPermissions.Ledgers.Default)]
    public class LedgerAppService : MwpAppService, ILedgerAppService
    {
        readonly IRepository<Ledger, Guid> _ledgerRepository;

        public LedgerAppService(IRepository<Ledger, Guid> ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        public virtual async Task<PagedResultDto<LedgerDto>> GetListAsync(GetLedgersInput input)
        {
            var totalCount = await _ledgerRepository.GetCountAsync();
            var items = await _ledgerRepository.GetListAsync();

            return new PagedResultDto<LedgerDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Ledger>, List<LedgerDto>>(items)
            };
        }

        public virtual async Task<LedgerDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Ledger, LedgerDto>(await _ledgerRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Ledgers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _ledgerRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Ledgers.Create)]
        public virtual async Task<LedgerDto> CreateAsync(LedgerCreateDto input)
        {
            var ledger = ObjectMapper.Map<LedgerCreateDto, Ledger>(input);
            ledger.TenantId = CurrentTenant.Id;
            ledger = await _ledgerRepository.InsertAsync(ledger, autoSave: true);
            return ObjectMapper.Map<Ledger, LedgerDto>(ledger);
        }

        [Authorize(MwpPermissions.Ledgers.Edit)]
        public virtual async Task<LedgerDto> UpdateAsync(Guid id, LedgerUpdateDto input)
        {
            var ledger = await _ledgerRepository.GetAsync(id);
            ObjectMapper.Map(input, ledger);
            ledger = await _ledgerRepository.UpdateAsync(ledger);
            return ObjectMapper.Map<Ledger, LedgerDto>(ledger);
        }
    }
}