using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Engagements
{
    public class DocumentAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IDocumentAppService _documentAppService;
        readonly IRepository<Document, Guid> _documentRepository;
        readonly MwpTestData _testData;

        public DocumentAppServiceTests()
        {
            _documentAppService = GetRequiredService<IDocumentAppService>();
            _documentRepository = GetRequiredService<IRepository<Document, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var document1 = new Document(_testData.Document1Id, null, MwpTestData.Document1Name, _testData.Workpaper1Id, _testData.Document1IsActive);
            var document2 = new Document(_testData.Document2Id, null, MwpTestData.Document2Name, _testData.Workpaper2Id, _testData.Document2IsActive);
            await _documentRepository.InsertAsync(document1);
            await _documentRepository.InsertAsync(document2);

            // Act
            var result = await _documentAppService.GetListAsync(new GetDocumentsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Document1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Document2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var document1 = new Document(_testData.Document1Id, null, MwpTestData.Document1Name, _testData.Workpaper1Id, _testData.Document1IsActive);
            var document2 = new Document(_testData.Document2Id, null, MwpTestData.Document2Name, _testData.Workpaper2Id, _testData.Document2IsActive);
            await _documentRepository.InsertAsync(document1);
            await _documentRepository.InsertAsync(document2);

            // Act
            var result = await _documentAppService.GetAsync(_testData.Document1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Document1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new DocumentCreateDto
            {
                Name = MwpTestData.Document2Name,
                IsActive = _testData.Document2IsActive
            };

            // Act
            var serviceResult = await _documentAppService.CreateAsync(input);

            // Assert
            var result = await _documentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Document2Name);
            result.IsActive.ShouldBe(_testData.Document2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var document1 = new Document(_testData.Document1Id, null, MwpTestData.Document1Name, _testData.Workpaper1Id, _testData.Document1IsActive);
            await _documentRepository.InsertAsync(document1);
            var input = new DocumentUpdateDto()
            {
                Name = MwpTestData.Document1Name,
                IsActive = _testData.Document1IsActive
            };

            // Act
            var serviceResult = await _documentAppService.UpdateAsync(_testData.Document1Id, input);

            // Assert
            var result = await _documentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Document1Name);
            result.IsActive.ShouldBe(_testData.Document1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var document2 = new Document(_testData.Document2Id, null, MwpTestData.Document2Name, _testData.Workpaper2Id, _testData.Document2IsActive);
            await _documentRepository.InsertAsync(document2);

            // Act
            await _documentAppService.DeleteAsync(_testData.Document2Id);

            // Assert
            var result = await _documentRepository.FindAsync(c => c.Id == _testData.Document2Id);

            result.ShouldBeNull();
        }
    }
}