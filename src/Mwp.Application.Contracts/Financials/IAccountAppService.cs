using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Financials
{
    public interface IAccountAppService : ICrudAppService<AccountDto, Guid, GetAccountsInput, AccountCreateDto, AccountUpdateDto>
    {
        Task<PagedResultDto<AccountExtendedDto>> GetExtendedListAsync(GetAccountsInput input);

        Task<AccountExtendedDto> GetExtendedAsync(Guid id);

        Task<List<AccountNameIdDto>> GetNameIdsAsync(string sorting = "name asc");
    }
}