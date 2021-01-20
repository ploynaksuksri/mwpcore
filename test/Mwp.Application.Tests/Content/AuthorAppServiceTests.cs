using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class AuthorAppServiceTests : MwpApplicationTestBase
    {
        readonly IAuthorAppService _authorAppService;
        readonly IRepository<Author, Guid> _authorRepository;
        readonly MwpTestData _testData;

        public AuthorAppServiceTests()
        {
            _authorAppService = GetRequiredService<IAuthorAppService>();
            _authorRepository = GetRequiredService<IRepository<Author, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var author1 = new Author(_testData.Author1Id, null, MwpTestData.Author1Name, _testData.Author1IsActive);
            var author2 = new Author(_testData.Author2Id, null, MwpTestData.Author2Name, _testData.Author2IsActive);
            await _authorRepository.InsertAsync(author1);
            await _authorRepository.InsertAsync(author2);

            // Act
            var result = await _authorAppService.GetListAsync(new GetAuthorsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Author1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Author2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var author1 = new Author(_testData.Author1Id, null, MwpTestData.Author1Name, _testData.Author1IsActive);
            var author2 = new Author(_testData.Author2Id, null, MwpTestData.Author2Name, _testData.Author2IsActive);
            await _authorRepository.InsertAsync(author1);
            await _authorRepository.InsertAsync(author2);

            // Act
            var result = await _authorAppService.GetAsync(_testData.Author1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Author1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new AuthorCreateDto
            {
                Name = MwpTestData.Author2Name,
                IsActive = _testData.Author2IsActive
            };

            // Act
            var serviceResult = await _authorAppService.CreateAsync(input);

            // Assert
            var result = await _authorRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Author2Name);
            result.IsActive.ShouldBe(_testData.Author2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var author1 = new Author(_testData.Author1Id, null, MwpTestData.Author1Name, _testData.Author1IsActive);
            await _authorRepository.InsertAsync(author1);
            var input = new AuthorUpdateDto()
            {
                Name = MwpTestData.Author1Name,
                IsActive = _testData.Author1IsActive
            };

            // Act
            var serviceResult = await _authorAppService.UpdateAsync(_testData.Author1Id, input);

            // Assert
            var result = await _authorRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Author1Name);
            result.IsActive.ShouldBe(_testData.Author1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var author1 = new Author(_testData.Author1Id, null, MwpTestData.Author1Name, _testData.Author1IsActive);
            await _authorRepository.InsertAsync(author1);

            // Act
            await _authorAppService.DeleteAsync(_testData.Author2Id);

            // Assert
            var result = await _authorRepository.FindAsync(c => c.Id == _testData.Author2Id);

            result.ShouldBeNull();
        }
    }
}