using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Engagements
{
    public class WorkpaperAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IWorkpaperAppService _workpaperAppService;
        readonly IRepository<Workpaper, Guid> _workpaperRepository;
        readonly MwpTestData _testData;

        public WorkpaperAppServiceTests()
        {
            _workpaperAppService = GetRequiredService<IWorkpaperAppService>();
            _workpaperRepository = GetRequiredService<IRepository<Workpaper, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var workpaper1 = new Workpaper(_testData.Workpaper1Id, null, MwpTestData.Workpaper1Name, _testData.Folder1Id, _testData.Workpaper1IsActive);
            var workpaper2 = new Workpaper(_testData.Workpaper2Id, null, MwpTestData.Workpaper2Name, _testData.Folder2Id, _testData.Workpaper2IsActive);
            await _workpaperRepository.InsertAsync(workpaper1);
            await _workpaperRepository.InsertAsync(workpaper2);

            // Act
            var result = await _workpaperAppService.GetListAsync(new GetWorkpapersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Workpaper1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Workpaper2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var workpaper1 = new Workpaper(_testData.Workpaper1Id, null, MwpTestData.Workpaper1Name, _testData.Folder1Id, _testData.Workpaper1IsActive);
            var workpaper2 = new Workpaper(_testData.Workpaper2Id, null, MwpTestData.Workpaper2Name, _testData.Folder2Id, _testData.Workpaper2IsActive);
            await _workpaperRepository.InsertAsync(workpaper1);
            await _workpaperRepository.InsertAsync(workpaper2);

            // Act
            var result = await _workpaperAppService.GetAsync(_testData.Workpaper1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Workpaper1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new WorkpaperCreateDto
            {
                Name = MwpTestData.Workpaper2Name,
                IsActive = _testData.Workpaper2IsActive
            };

            // Act
            var serviceResult = await _workpaperAppService.CreateAsync(input);

            // Assert
            var result = await _workpaperRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Workpaper2Name);
            result.IsActive.ShouldBe(_testData.Workpaper2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var workpaper1 = new Workpaper(_testData.Workpaper1Id, null, MwpTestData.Workpaper1Name, _testData.Folder1Id, _testData.Workpaper1IsActive);
            await _workpaperRepository.InsertAsync(workpaper1);
            var input = new WorkpaperUpdateDto()
            {
                Name = MwpTestData.Workpaper1Name,
                IsActive = _testData.Workpaper1IsActive
            };

            // Act
            var serviceResult = await _workpaperAppService.UpdateAsync(_testData.Workpaper1Id, input);

            // Assert
            var result = await _workpaperRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Workpaper1Name);
            result.IsActive.ShouldBe(_testData.Workpaper1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var workpaper2 = new Workpaper(_testData.Workpaper2Id, null, MwpTestData.Workpaper2Name, _testData.Folder2Id, _testData.Workpaper2IsActive);
            await _workpaperRepository.InsertAsync(workpaper2);

            // Act
            await _workpaperAppService.DeleteAsync(_testData.Workpaper2Id);

            // Assert
            var result = await _workpaperRepository.FindAsync(c => c.Id == _testData.Workpaper2Id);

            result.ShouldBeNull();
        }
    }
}