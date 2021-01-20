using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class TitleAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly ITitleAppService _titleAppService;
        readonly IRepository<Title, Guid> _titleRepository;
        readonly MwpTestData _testData;

        public TitleAppServiceTests()
        {
            _titleAppService = GetRequiredService<ITitleAppService>();
            _titleRepository = GetRequiredService<IRepository<Title, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var title1 = new Title(_testData.Title1Id, null, MwpTestData.Title1Name, _testData.TitleCategory1Id, _testData.Workbook1Id, _testData.Publisher1Id, _testData.Title1IsActive);
            var title2 = new Title(_testData.Title2Id, null, MwpTestData.Title2Name, _testData.TitleCategory2Id, _testData.Workbook2Id, _testData.Publisher2Id, _testData.Title2IsActive);
            await _titleRepository.InsertAsync(title1);
            await _titleRepository.InsertAsync(title2);

            // Act
            var result = await _titleAppService.GetListAsync(new GetTitlesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Title1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Title2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var title1 = new Title(_testData.Title1Id, null, MwpTestData.Title1Name, _testData.TitleCategory1Id, _testData.Workbook1Id, _testData.Publisher1Id, _testData.Title1IsActive);
            var title2 = new Title(_testData.Title2Id, null, MwpTestData.Title2Name, _testData.TitleCategory2Id, _testData.Workbook2Id, _testData.Publisher2Id, _testData.Title2IsActive);
            await _titleRepository.InsertAsync(title1);
            await _titleRepository.InsertAsync(title2);

            // Act
            var result = await _titleAppService.GetAsync(_testData.Title1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Title1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TitleCreateDto
            {
                Name = MwpTestData.Title2Name,
                IsActive = _testData.Title2IsActive
            };

            // Act
            var serviceResult = await _titleAppService.CreateAsync(input);

            // Assert
            var result = await _titleRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Title2Name);
            result.IsActive.ShouldBe(_testData.Title2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var title1 = new Title(_testData.Title1Id, null, MwpTestData.Title1Name, _testData.TitleCategory1Id, _testData.Workbook1Id, _testData.Publisher1Id, _testData.Title1IsActive);
            await _titleRepository.InsertAsync(title1);
            var input = new TitleUpdateDto()
            {
                Name = MwpTestData.Title1Name,
                IsActive = _testData.Title1IsActive
            };

            // Act
            var serviceResult = await _titleAppService.UpdateAsync(_testData.Title1Id, input);

            // Assert
            var result = await _titleRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Title1Name);
            result.IsActive.ShouldBe(_testData.Title1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var title2 = new Title(_testData.Title2Id, null, MwpTestData.Title2Name, _testData.TitleCategory2Id, _testData.Workbook2Id, _testData.Publisher2Id, _testData.Title2IsActive);
            await _titleRepository.InsertAsync(title2);

            // Act
            await _titleAppService.DeleteAsync(_testData.Title2Id);

            // Assert
            var result = await _titleRepository.FindAsync(c => c.Id == _testData.Title2Id);

            result.ShouldBeNull();
        }
    }
}