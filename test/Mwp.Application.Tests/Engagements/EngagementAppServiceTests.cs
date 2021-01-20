using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Engagements
{
    public class EngagementAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IEngagementAppService _engagementAppService;
        readonly IRepository<Engagement, Guid> _engagementRepository;
        readonly MwpTestData _testData;

        public EngagementAppServiceTests()
        {
            _engagementAppService = GetRequiredService<IEngagementAppService>();
            _engagementRepository = GetRequiredService<IRepository<Engagement, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var engagement1 = new Engagement(_testData.Engagement1Id, null, MwpTestData.Engagement1Name, _testData.Entity1Id, _testData.Engagement1IsActive);
            var engagement2 = new Engagement(_testData.Engagement2Id, null, MwpTestData.Engagement2Name, _testData.Entity2Id, _testData.Engagement2IsActive);
            await _engagementRepository.InsertAsync(engagement1);
            await _engagementRepository.InsertAsync(engagement2);

            // Act
            var result = await _engagementAppService.GetListAsync(new GetEngagementsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Engagement1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Engagement2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var engagement1 = new Engagement(_testData.Engagement1Id, null, MwpTestData.Engagement1Name, _testData.Entity1Id, _testData.Engagement1IsActive);
            var engagement2 = new Engagement(_testData.Engagement2Id, null, MwpTestData.Engagement2Name, _testData.Entity2Id, _testData.Engagement2IsActive);
            await _engagementRepository.InsertAsync(engagement1);
            await _engagementRepository.InsertAsync(engagement2);

            // Act
            var result = await _engagementAppService.GetAsync(_testData.Engagement1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Engagement1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EngagementCreateDto
            {
                Name = MwpTestData.Engagement2Name,
                IsActive = _testData.Engagement2IsActive
            };

            // Act
            var serviceResult = await _engagementAppService.CreateAsync(input);

            // Assert
            var result = await _engagementRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Engagement2Name);
            result.IsActive.ShouldBe(_testData.Engagement2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var engagement1 = new Engagement(_testData.Engagement1Id, null, MwpTestData.Engagement1Name, _testData.Entity1Id, _testData.Engagement1IsActive);
            await _engagementRepository.InsertAsync(engagement1);
            var input = new EngagementUpdateDto()
            {
                Name = MwpTestData.Engagement1Name,
                IsActive = _testData.Engagement1IsActive
            };

            // Act
            var serviceResult = await _engagementAppService.UpdateAsync(_testData.Engagement1Id, input);

            // Assert
            var result = await _engagementRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Engagement1Name);
            result.IsActive.ShouldBe(_testData.Engagement1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var engagement2 = new Engagement(_testData.Engagement2Id, null, MwpTestData.Engagement2Name, _testData.Entity2Id, _testData.Engagement2IsActive);
            await _engagementRepository.InsertAsync(engagement2);

            // Act
            await _engagementAppService.DeleteAsync(_testData.Engagement2Id);

            // Assert
            var result = await _engagementRepository.FindAsync(c => c.Id == _testData.Engagement2Id);

            result.ShouldBeNull();
        }
    }
}