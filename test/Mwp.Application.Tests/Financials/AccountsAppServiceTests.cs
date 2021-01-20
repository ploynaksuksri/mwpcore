using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Xunit;

namespace Mwp.Financials
{
    public class AccountsAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IAccountAppService _accountAppService;
        readonly IAccountRepository _accountRepository;
        readonly MwpTestData _testData;

        public AccountsAppServiceTests()
        {
            _accountAppService = GetRequiredService<IAccountAppService>();
            _accountRepository = GetRequiredService<IAccountRepository>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var account1 = new Account(_testData.Account1Id, null, "Microsoft", "Microsoft Skype", "info@skype.com", "+1 555 55 555", _testData.ParentAccount1Id, _testData.Ledger1Id);
            var account2 = new Account(_testData.Account2Id, null, "Amazon", "Amazon AWS", "info@aws.com", "+1 777 77 777", _testData.ParentAccount2Id, _testData.Ledger2Id);
            await _accountRepository.InsertAsync(account1);
            await _accountRepository.InsertAsync(account2);

            // Act
            var list = await _accountAppService.GetListAsync(new GetAccountsInput());

            // Assert
            list.TotalCount.ShouldBe(2);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var account1 = new Account(_testData.Account1Id, null, "Microsoft", "Microsoft Skype", "info@skype.com", "+1 555 55 555", _testData.ParentAccount1Id, _testData.Ledger1Id);
            var account2 = new Account(_testData.Account2Id, null, "Amazon", "Amazon AWS", "info@aws.com", "+1 777 77 777", _testData.ParentAccount2Id, _testData.Ledger2Id);
            await _accountRepository.InsertAsync(account1);
            await _accountRepository.InsertAsync(account2);

            // Act
            var account = await _accountAppService.GetAsync(_testData.Account1Id);

            // Assert
            account.ShouldNotBeNull();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetExtendedAsync()
        {
            // Arrange
            var account1 = new Account(_testData.Account1Id, null, "Microsoft", "Microsoft Skype", "info@skype.com", "+1 555 55 555", _testData.ParentAccount1Id, _testData.Ledger1Id);
            var account2 = new Account(_testData.Account2Id, null, "Amazon", "Amazon AWS", "info@aws.com", "+1 777 77 777", _testData.ParentAccount2Id, _testData.Ledger2Id);
            await _accountRepository.InsertAsync(account1);
            await _accountRepository.InsertAsync(account2);

            // Act
            var account = await _accountAppService.GetExtendedAsync(_testData.Account1Id);

            // Assert
            account.ShouldNotBeNull();
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var account1 = new Account(_testData.Account1Id, null, "Microsoft", "Microsoft Skype", "info@skype.com", "+1 555 55 555", _testData.ParentAccount1Id, _testData.Ledger1Id);
            await _accountRepository.InsertAsync(account1);

            // Act
            await _accountAppService.DeleteAsync(_testData.Account1Id);

            // Assert
            (await _accountRepository.FindAsync(_testData.Account1Id))
                .ShouldBeNull();
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var account = new AccountCreateDto()
            {
                CountryId = _testData.Country1Id,
                EmailAddress = "test1@test.com",
                FullName = "Volosoft ABP",
                Name = "Volosoft"
            };

            // Act
            var createdAccount = await _accountAppService.CreateAsync(account);

            // Assert
            createdAccount.Id.ShouldNotBeNull();
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var account1 = new Account(_testData.Account1Id, null, "Microsoft", "Microsoft Skype", "info@skype.com", "+1 555 55 555", _testData.ParentAccount1Id, _testData.Ledger1Id);
            await _accountRepository.InsertAsync(account1);
            const string newName = "It was Microsoft Skype";
            var updateDto = new AccountUpdateDto()
            {
                CountryId = _testData.Country1Id,
                EmailAddress = "info@skype.com",
                FullName = newName,
                Name = "Microsoft"
            };

            // Act
            await _accountAppService.UpdateAsync(_testData.Account1Id, updateDto);

            // Assert
            (await _accountAppService.GetAsync(_testData.Account1Id)).FullName.ShouldBe(newName);
        }
    }
}