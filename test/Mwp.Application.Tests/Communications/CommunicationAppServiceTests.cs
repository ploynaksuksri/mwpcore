using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Communications
{
    public class CommunicationAppServiceTests : MwpApplicationTestBase
    {
        readonly ICommunicationAppService _communicationAppService;
        readonly IRepository<Communication, Guid> _communicationRepository;
        readonly MwpTestData _testData;

        public CommunicationAppServiceTests()
        {
            _communicationAppService = GetRequiredService<ICommunicationAppService>();
            _communicationRepository = GetRequiredService<IRepository<Communication, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var communication1 = new Communication(_testData.Communication1Id, null, MwpTestData.Communication1Name, _testData.Communication1IsActive);
            var communication2 = new Communication(_testData.Communication2Id, null, MwpTestData.Communication2Name, _testData.Communication2IsActive);
            await _communicationRepository.InsertAsync(communication1);
            await _communicationRepository.InsertAsync(communication2);

            // Act
            var result = await _communicationAppService.GetListAsync(new GetCommunicationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Communication1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Communication2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var communication1 = new Communication(_testData.Communication1Id, null, MwpTestData.Communication1Name, _testData.Communication1IsActive);
            var communication2 = new Communication(_testData.Communication2Id, null, MwpTestData.Communication2Name, _testData.Communication2IsActive);
            await _communicationRepository.InsertAsync(communication1);
            await _communicationRepository.InsertAsync(communication2);

            // Act
            var result = await _communicationAppService.GetAsync(_testData.Communication1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Communication1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new CommunicationCreateDto
            {
                Name = MwpTestData.Communication2Name,
                IsActive = _testData.Communication2IsActive
            };

            // Act
            var serviceResult = await _communicationAppService.CreateAsync(input);

            // Assert
            var result = await _communicationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Communication2Name);
            result.IsActive.ShouldBe(_testData.Communication2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var communication1 = new Communication(_testData.Communication1Id, null, MwpTestData.Communication1Name, _testData.Communication1IsActive);
            await _communicationRepository.InsertAsync(communication1);
            var input = new CommunicationUpdateDto()
            {
                Name = MwpTestData.Communication1Name,
                IsActive = _testData.Communication1IsActive
            };

            // Act
            var serviceResult = await _communicationAppService.UpdateAsync(_testData.Communication1Id, input);

            // Assert
            var result = await _communicationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Communication1Name);
            result.IsActive.ShouldBe(_testData.Communication1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var communication2 = new Communication(_testData.Communication2Id, null, MwpTestData.Communication2Name, _testData.Communication2IsActive);
            await _communicationRepository.InsertAsync(communication2);

            // Act
            await _communicationAppService.DeleteAsync(_testData.Communication2Id);

            // Assert
            var result = await _communicationRepository.FindAsync(c => c.Id == _testData.Communication2Id);

            result.ShouldBeNull();
        }
    }
}