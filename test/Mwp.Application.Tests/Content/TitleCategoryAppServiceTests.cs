using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class TitleCategoryAppServiceTests : MwpApplicationTestBase
    {
        readonly ITitleCategoryAppService _titleCategoryAppService;
        readonly IRepository<TitleCategory, Guid> _titleCategoryRepository;
        readonly MwpTestData _testData;

        public TitleCategoryAppServiceTests()
        {
            _titleCategoryAppService = GetRequiredService<ITitleCategoryAppService>();
            _titleCategoryRepository = GetRequiredService<IRepository<TitleCategory, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var titleCategory1 = new TitleCategory(_testData.TitleCategory1Id, null, MwpTestData.TitleCategory1Name, _testData.TitleCategory1IsActive);
            var titleCategory2 = new TitleCategory(_testData.TitleCategory2Id, null, MwpTestData.TitleCategory2Name, _testData.TitleCategory2IsActive);
            await _titleCategoryRepository.InsertAsync(titleCategory1);
            await _titleCategoryRepository.InsertAsync(titleCategory2);

            // Act
            var result = await _titleCategoryAppService.GetListAsync(new GetTitleCategoriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.TitleCategory1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.TitleCategory2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var titleCategory1 = new TitleCategory(_testData.TitleCategory1Id, null, MwpTestData.TitleCategory1Name, _testData.TitleCategory1IsActive);
            var titleCategory2 = new TitleCategory(_testData.TitleCategory2Id, null, MwpTestData.TitleCategory2Name, _testData.TitleCategory2IsActive);
            await _titleCategoryRepository.InsertAsync(titleCategory1);
            await _titleCategoryRepository.InsertAsync(titleCategory2);

            // Act
            var result = await _titleCategoryAppService.GetAsync(_testData.TitleCategory1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.TitleCategory1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TitleCategoryCreateDto
            {
                Name = MwpTestData.TitleCategory2Name,
                IsActive = _testData.TitleCategory2IsActive
            };

            // Act
            var serviceResult = await _titleCategoryAppService.CreateAsync(input);

            // Assert
            var result = await _titleCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.TitleCategory2Name);
            result.IsActive.ShouldBe(_testData.TitleCategory2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var titleCategory1 = new TitleCategory(_testData.TitleCategory1Id, null, MwpTestData.TitleCategory1Name, _testData.TitleCategory1IsActive);
            await _titleCategoryRepository.InsertAsync(titleCategory1);
            var input = new TitleCategoryUpdateDto()
            {
                Name = MwpTestData.TitleCategory1Name,
                IsActive = _testData.TitleCategory1IsActive
            };

            // Act
            var serviceResult = await _titleCategoryAppService.UpdateAsync(_testData.TitleCategory1Id, input);

            // Assert
            var result = await _titleCategoryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.TitleCategory1Name);
            result.IsActive.ShouldBe(_testData.TitleCategory1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var titleCategory2 = new TitleCategory(_testData.TitleCategory2Id, null, MwpTestData.TitleCategory2Name, _testData.TitleCategory2IsActive);
            await _titleCategoryRepository.InsertAsync(titleCategory2);

            // Act
            await _titleCategoryAppService.DeleteAsync(_testData.TitleCategory2Id);

            // Assert
            var result = await _titleCategoryRepository.FindAsync(c => c.Id == _testData.TitleCategory2Id);

            result.ShouldBeNull();
        }
    }
}