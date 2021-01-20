using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mwp.Financials;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Mwp.Controllers
{
    [RemoteService(Name = "AbpAccount")]
    [Area("account")]
    [Route("api/account")]
    public class AccountController : MwpController, IAccountAppService
    {
        readonly IAccountAppService _accountAppService;

        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        [HttpGet]
        [Route("list")]
        public async Task<PagedResultDto<AccountDto>> GetListAsync(GetAccountsInput input)
        {
            return await _accountAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("extended-list")]
        public async Task<PagedResultDto<AccountExtendedDto>> GetExtendedListAsync(GetAccountsInput input)
        {
            return await _accountAppService.GetExtendedListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<AccountDto> GetAsync(Guid id)
        {
            return await _accountAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("{id}/extended")]
        public async Task<AccountExtendedDto> GetExtendedAsync(Guid id)
        {
            return await _accountAppService.GetExtendedAsync(id);
        }

        [HttpDelete]
        public async Task DeleteAsync(Guid id)
        {
            await _accountAppService.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            return await _accountAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}/account")]
        public async Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            return await _accountAppService.UpdateAsync(id, input);
        }

        [HttpGet]
        [Route("by-nameIds/{sorting}")]
        public async Task<List<AccountNameIdDto>> GetNameIdsAsync(string sorting = "name asc")
        {
            return await _accountAppService.GetNameIdsAsync(sorting);
        }
    }
}