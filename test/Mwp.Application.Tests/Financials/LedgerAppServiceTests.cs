using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Financials
{
    public class LedgerAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly ILedgerAppService _ledgerAppService;
        readonly IRepository<Ledger, Guid> _ledgerRepository;
        readonly MwpTestData _testData;

        public LedgerAppServiceTests()
        {
            _ledgerAppService = GetRequiredService<ILedgerAppService>();
            _ledgerRepository = GetRequiredService<IRepository<Ledger, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var ledger1 = new Ledger(_testData.Ledger1Id, null, MwpTestData.Ledger1Name, _testData.Entity1Id, _testData.Ledger1IsActive);
            var ledger2 = new Ledger(_testData.Ledger2Id, null, MwpTestData.Ledger2Name, _testData.Entity2Id, _testData.Ledger2IsActive);
            await _ledgerRepository.InsertAsync(ledger1);
            await _ledgerRepository.InsertAsync(ledger2);

            // Act
            var result = await _ledgerAppService.GetListAsync(new GetLedgersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Ledger1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Ledger2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var ledger1 = new Ledger(_testData.Ledger1Id, null, MwpTestData.Ledger1Name, _testData.Entity1Id, _testData.Ledger1IsActive);
            await _ledgerRepository.InsertAsync(ledger1);

            // Act
            var result = await _ledgerAppService.GetAsync(_testData.Ledger1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Ledger1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new LedgerCreateDto
            {
                Name = MwpTestData.Ledger2Name,
                IsActive = _testData.Ledger2IsActive
            };

            // Act
            var serviceResult = await _ledgerAppService.CreateAsync(input);

            // Assert
            var result = await _ledgerRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Ledger2Name);
            result.IsActive.ShouldBe(_testData.Ledger2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var ledger1 = new Ledger(_testData.Ledger1Id, null, MwpTestData.Ledger1Name, _testData.Entity1Id, _testData.Ledger1IsActive);
            await _ledgerRepository.InsertAsync(ledger1);
            var input = new LedgerUpdateDto()
            {
                Name = MwpTestData.Ledger1Name,
                IsActive = _testData.Ledger1IsActive
            };

            // Act
            var serviceResult = await _ledgerAppService.UpdateAsync(_testData.Ledger1Id, input);

            // Assert
            var result = await _ledgerRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Ledger1Name);
            result.IsActive.ShouldBe(_testData.Ledger1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var ledger2 = new Ledger(_testData.Ledger2Id, null, MwpTestData.Ledger2Name, _testData.Entity2Id, _testData.Ledger2IsActive);
            await _ledgerRepository.InsertAsync(ledger2);

            // Act
            await _ledgerAppService.DeleteAsync(_testData.Ledger2Id);

            // Assert
            var result = await _ledgerRepository.FindAsync(c => c.Id == _testData.Ledger2Id);

            result.ShouldBeNull();
        }
    }
}