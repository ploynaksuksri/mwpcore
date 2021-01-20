using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;

namespace Mwp.Financials
{
    [Authorize(MwpPermissions.Accounts.Default)]
    public class AccountAppService : MwpAppService, IAccountAppService
    {
        readonly IAccountRepository _accountRepository;

        public AccountAppService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [Authorize(MwpPermissions.Accounts.Default)]
        public virtual async Task<PagedResultDto<AccountDto>> GetListAsync(GetAccountsInput input)
        {
            return await GetAccountListAsync<AccountDto>(input);
        }

        [Authorize(MwpPermissions.Accounts.Default)]
        public virtual async Task<PagedResultDto<AccountExtendedDto>> GetExtendedListAsync(GetAccountsInput input)
        {
            return await GetAccountListAsync<AccountExtendedDto>(input, true);
        }

        [Authorize(MwpPermissions.Accounts.Default)]
        public virtual async Task<AccountDto> GetAsync(Guid id)
        {
            return await GetAccountAsync<AccountDto>(id);
        }

        [Authorize(MwpPermissions.Accounts.Default)]
        public virtual async Task<AccountExtendedDto> GetExtendedAsync(Guid id)
        {
            return await GetAccountAsync<AccountExtendedDto>(id, true);
        }

        [Authorize(MwpPermissions.Accounts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Accounts.Create)]
        public virtual async Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            var newAccount = ObjectMapper.Map<AccountCreateDto, Account>(input);
            newAccount.TenantId = CurrentTenant.Id;
            var account = await _accountRepository.InsertAsync(newAccount);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<Account, AccountDto>(account);
        }

        [Authorize(MwpPermissions.Accounts.Edit)]
        public virtual async Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            var account = await _accountRepository.GetAsync(id);
            ObjectMapper.Map(input, account);
            var updatedAccount = await _accountRepository.UpdateAsync(account);
            return ObjectMapper.Map<Account, AccountDto>(updatedAccount);
        }

        public virtual async Task<List<AccountNameIdDto>> GetNameIdsAsync(string sorting = "fullName asc")
        {
            var accounts = await _accountRepository.GetListAsync(sorting: sorting);
            return ObjectMapper.Map<List<Account>, List<AccountNameIdDto>>(accounts);
        }

        async Task<PagedResultDto<TAccountDtoType>> GetAccountListAsync<TAccountDtoType>(GetAccountsInput input, bool isExtended = false) where TAccountDtoType : AccountDto
        {
            input.CountryId = input.CountryId == null || input.CountryId == Guid.Empty ? null : input.CountryId;

            var totalCount = await _accountRepository.GetCountAsync(input.FilterText, input.Name, input.FullName, input.EmailAddress, input.PhoneNumber);
            var items = await _accountRepository.GetListAsync(input.FilterText, input.Name, input.FullName, input.EmailAddress, input.PhoneNumber, input.Sorting, input.MaxResultCount, input.SkipCount, isExtended);

            return new PagedResultDto<TAccountDtoType>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Account>, List<TAccountDtoType>>(items)
            };
        }

        async Task<TAccountDtoType> GetAccountAsync<TAccountDtoType>(Guid id, bool isExtended = false) where TAccountDtoType : AccountDto
        {
            return ObjectMapper.Map<Account, TAccountDtoType>(await _accountRepository.GetAsync(id, isExtended));
        }
    }
}