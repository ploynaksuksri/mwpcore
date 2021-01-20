using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class PublisherAppServiceTests : MwpApplicationTestBase
    {
        readonly IPublisherAppService _publisherAppService;
        readonly IRepository<Publisher, Guid> _publisherRepository;
        readonly MwpTestData _testData;

        public PublisherAppServiceTests()
        {
            _publisherAppService = GetRequiredService<IPublisherAppService>();
            _publisherRepository = GetRequiredService<IRepository<Publisher, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var publisher1 = new Publisher(_testData.Publisher1Id, null, MwpTestData.Publisher1Name, _testData.Publisher1IsActive);
            var publisher2 = new Publisher(_testData.Publisher2Id, null, MwpTestData.Publisher2Name, _testData.Publisher2IsActive);
            await _publisherRepository.InsertAsync(publisher1);
            await _publisherRepository.InsertAsync(publisher2);

            // Act
            var result = await _publisherAppService.GetListAsync(new GetPublishersInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Publisher1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Publisher2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var publisher1 = new Publisher(_testData.Publisher1Id, null, MwpTestData.Publisher1Name, _testData.Publisher1IsActive);
            var publisher2 = new Publisher(_testData.Publisher2Id, null, MwpTestData.Publisher2Name, _testData.Publisher2IsActive);
            await _publisherRepository.InsertAsync(publisher1);
            await _publisherRepository.InsertAsync(publisher2);

            // Act
            var result = await _publisherAppService.GetAsync(_testData.Publisher1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Publisher1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PublisherCreateDto
            {
                Name = MwpTestData.Publisher2Name,
                IsActive = _testData.Publisher2IsActive
            };

            // Act
            var serviceResult = await _publisherAppService.CreateAsync(input);

            // Assert
            var result = await _publisherRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Publisher2Name);
            result.IsActive.ShouldBe(_testData.Publisher2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var publisher1 = new Publisher(_testData.Publisher1Id, null, MwpTestData.Publisher1Name, _testData.Publisher1IsActive);
            await _publisherRepository.InsertAsync(publisher1);
            var input = new PublisherUpdateDto()
            {
                Name = MwpTestData.Publisher1Name,
                IsActive = _testData.Publisher1IsActive
            };

            // Act
            var serviceResult = await _publisherAppService.UpdateAsync(_testData.Publisher1Id, input);

            // Assert
            var result = await _publisherRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Publisher1Name);
            result.IsActive.ShouldBe(_testData.Publisher1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var publisher2 = new Publisher(_testData.Publisher2Id, null, MwpTestData.Publisher2Name, _testData.Publisher2IsActive);
            await _publisherRepository.InsertAsync(publisher2);

            // Act
            await _publisherAppService.DeleteAsync(_testData.Publisher2Id);

            // Assert
            var result = await _publisherRepository.FindAsync(c => c.Id == _testData.Publisher2Id);

            result.ShouldBeNull();
        }
    }
}