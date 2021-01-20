using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Engagements
{
    public class WorkbookAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IWorkbookAppService _workbookAppService;
        readonly IRepository<Workbook, Guid> _workbookRepository;
        readonly MwpTestData _testData;

        public WorkbookAppServiceTests()
        {
            _workbookAppService = GetRequiredService<IWorkbookAppService>();
            _workbookRepository = GetRequiredService<IRepository<Workbook, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var workbook1 = new Workbook(_testData.Workbook1Id, null, MwpTestData.Workbook1Name, _testData.Engagement1Id, _testData.Workbook1IsActive);
            var workbook2 = new Workbook(_testData.Workbook2Id, null, MwpTestData.Workbook2Name, _testData.Engagement2Id, _testData.Workbook2IsActive);
            await _workbookRepository.InsertAsync(workbook1);
            await _workbookRepository.InsertAsync(workbook2);

            // Act
            var result = await _workbookAppService.GetListAsync(new GetWorkbooksInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Workbook1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Workbook2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var workbook1 = new Workbook(_testData.Workbook1Id, null, MwpTestData.Workbook1Name, _testData.Engagement1Id, _testData.Workbook1IsActive);
            var workbook2 = new Workbook(_testData.Workbook2Id, null, MwpTestData.Workbook2Name, _testData.Engagement2Id, _testData.Workbook2IsActive);
            await _workbookRepository.InsertAsync(workbook1);
            await _workbookRepository.InsertAsync(workbook2);

            // Act
            var result = await _workbookAppService.GetAsync(_testData.Workbook1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Workbook1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new WorkbookCreateDto
            {
                Name = MwpTestData.Workbook2Name,
                IsActive = _testData.Workbook2IsActive
            };

            // Act
            var serviceResult = await _workbookAppService.CreateAsync(input);

            // Assert
            var result = await _workbookRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Workbook2Name);
            result.IsActive.ShouldBe(_testData.Workbook2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var workbook1 = new Workbook(_testData.Workbook1Id, null, MwpTestData.Workbook1Name, _testData.Engagement1Id, _testData.Workbook1IsActive);
            await _workbookRepository.InsertAsync(workbook1);
            var input = new WorkbookUpdateDto()
            {
                Name = MwpTestData.Workbook1Name,
                IsActive = _testData.Workbook1IsActive
            };

            // Act
            var serviceResult = await _workbookAppService.UpdateAsync(_testData.Workbook1Id, input);

            // Assert
            var result = await _workbookRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Workbook1Name);
            result.IsActive.ShouldBe(_testData.Workbook1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var workbook2 = new Workbook(_testData.Workbook2Id, null, MwpTestData.Workbook2Name, _testData.Engagement2Id, _testData.Workbook2IsActive);
            await _workbookRepository.InsertAsync(workbook2);

            // Act
            await _workbookAppService.DeleteAsync(_testData.Workbook2Id);

            // Assert
            var result = await _workbookRepository.FindAsync(c => c.Id == _testData.Workbook2Id);

            result.ShouldBeNull();
        }
    }
}