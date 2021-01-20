using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Engagements
{
    public class FolderAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IFolderAppService _folderAppService;
        readonly IRepository<Folder, Guid> _folderRepository;
        readonly MwpTestData _testData;

        public FolderAppServiceTests()
        {
            _folderAppService = GetRequiredService<IFolderAppService>();
            _folderRepository = GetRequiredService<IRepository<Folder, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var folder1 = new Folder(_testData.Folder1Id, null, MwpTestData.Folder1Name, _testData.ParentFolder1Id, _testData.Workbook1Id, _testData.Folder1IsActive);
            var folder2 = new Folder(_testData.Folder2Id, null, MwpTestData.Folder2Name, _testData.ParentFolder2Id, _testData.Workbook2Id, _testData.Folder2IsActive);
            await _folderRepository.InsertAsync(folder1);
            await _folderRepository.InsertAsync(folder2);

            // Act
            var result = await _folderAppService.GetListAsync(new GetFoldersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Folder1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Folder2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var folder1 = new Folder(_testData.Folder1Id, null, MwpTestData.Folder1Name, _testData.ParentFolder1Id, _testData.Workbook1Id, _testData.Folder1IsActive);
            var folder2 = new Folder(_testData.Folder2Id, null, MwpTestData.Folder2Name, _testData.ParentFolder2Id, _testData.Workbook2Id, _testData.Folder2IsActive);
            await _folderRepository.InsertAsync(folder1);
            await _folderRepository.InsertAsync(folder2);

            // Act
            var result = await _folderAppService.GetAsync(_testData.Folder1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Folder1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new FolderCreateDto
            {
                Name = MwpTestData.Folder2Name,
                IsActive = _testData.Folder2IsActive
            };

            // Act
            var serviceResult = await _folderAppService.CreateAsync(input);

            // Assert
            var result = await _folderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Folder2Name);
            result.IsActive.ShouldBe(_testData.Folder2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var folder1 = new Folder(_testData.Folder1Id, null, MwpTestData.Folder1Name, _testData.ParentFolder1Id, _testData.Workbook1Id, _testData.Folder1IsActive);
            await _folderRepository.InsertAsync(folder1);
            var input = new FolderUpdateDto()
            {
                Name = MwpTestData.Folder1Name,
                IsActive = _testData.Folder1IsActive
            };

            // Act
            var serviceResult = await _folderAppService.UpdateAsync(_testData.Folder1Id, input);

            // Assert
            var result = await _folderRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Folder1Name);
            result.IsActive.ShouldBe(_testData.Folder1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var folder2 = new Folder(_testData.Folder2Id, null, MwpTestData.Folder2Name, _testData.ParentFolder2Id, _testData.Workbook2Id, _testData.Folder2IsActive);
            await _folderRepository.InsertAsync(folder2);

            // Act
            await _folderAppService.DeleteAsync(_testData.Folder2Id);

            // Assert
            var result = await _folderRepository.FindAsync(c => c.Id == _testData.Folder2Id);

            result.ShouldBeNull();
        }
    }
}