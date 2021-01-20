using System;
using Volo.Abp.Application.Services;

namespace Mwp.Financials
{
    public interface ILedgerAppService : ICrudAppService<LedgerDto, Guid, GetLedgersInput, LedgerCreateDto, LedgerUpdateDto>
    {
    }
}