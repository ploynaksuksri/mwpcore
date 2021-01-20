using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Communications
{
    public class WebsiteAppServiceTests : MwpApplicationTestBase
    {
        readonly IWebsiteAppService _websiteAppService;
        readonly IRepository<Website, Guid> _websiteRepository;
        readonly MwpTestData _testData;

        public WebsiteAppServiceTests()
        {
            _websiteAppService = GetRequiredService<IWebsiteAppService>();
            _websiteRepository = GetRequiredService<IRepository<Website, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var website1 = new Website(_testData.Website1Id, null, MwpTestData.Website1Name, _testData.Website1IsActive);
            var website2 = new Website(_testData.Website2Id, null, MwpTestData.Website2Name, _testData.Website2IsActive);
            await _websiteRepository.InsertAsync(website1);
            await _websiteRepository.InsertAsync(website2);

            // Act
            var result = await _websiteAppService.GetListAsync(new GetWebsitesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Website1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Website2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var website1 = new Website(_testData.Website1Id, null, MwpTestData.Website1Name, _testData.Website1IsActive);
            var website2 = new Website(_testData.Website2Id, null, MwpTestData.Website2Name, _testData.Website2IsActive);
            await _websiteRepository.InsertAsync(website1);
            await _websiteRepository.InsertAsync(website2);

            // Act
            var result = await _websiteAppService.GetAsync(_testData.Website1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Website1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new WebsiteCreateDto
            {
                Name = MwpTestData.Website2Name,
                IsActive = _testData.Website2IsActive
            };

            // Act
            var serviceResult = await _websiteAppService.CreateAsync(input);

            // Assert
            var result = await _websiteRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Website2Name);
            result.IsActive.ShouldBe(_testData.Website2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var website1 = new Website(_testData.Website1Id, null, MwpTestData.Website1Name, _testData.Website1IsActive);
            await _websiteRepository.InsertAsync(website1);
            var input = new WebsiteUpdateDto()
            {
                Name = MwpTestData.Website1Name,
                IsActive = _testData.Website1IsActive
            };

            // Act
            var serviceResult = await _websiteAppService.UpdateAsync(_testData.Website1Id, input);

            // Assert
            var result = await _websiteRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Website1Name);
            result.IsActive.ShouldBe(_testData.Website1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var website1 = new Website(_testData.Website1Id, null, MwpTestData.Website1Name, _testData.Website1IsActive);
            await _websiteRepository.InsertAsync(website1);

            // Act
            await _websiteAppService.DeleteAsync(_testData.Website2Id);

            // Assert
            var result = await _websiteRepository.FindAsync(c => c.Id == _testData.Website2Id);

            result.ShouldBeNull();
        }
    }
}