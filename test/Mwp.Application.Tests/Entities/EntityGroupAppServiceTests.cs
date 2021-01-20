using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Entities
{
    public class EntityGroupAppServiceTests : MwpApplicationTestBase
    {
        readonly IEntityGroupAppService _entityGroupAppService;
        readonly IRepository<EntityGroup, Guid> _entityGroupRepository;
        readonly MwpTestData _testData;

        public EntityGroupAppServiceTests()
        {
            _entityGroupAppService = GetRequiredService<IEntityGroupAppService>();
            _entityGroupRepository = GetRequiredService<IRepository<EntityGroup, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var entityGroup1 = new EntityGroup(_testData.EntityGroup1Id, null, MwpTestData.EntityGroup1Name, _testData.EntityGroup1IsActive);
            var entityGroup2 = new EntityGroup(_testData.EntityGroup2Id, null, MwpTestData.EntityGroup2Name, _testData.EntityGroup2IsActive);
            await _entityGroupRepository.InsertAsync(entityGroup1);
            await _entityGroupRepository.InsertAsync(entityGroup2);

            // Act
            var result = await _entityGroupAppService.GetListAsync(new GetEntityGroupsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.EntityGroup1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.EntityGroup2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var entityGroup1 = new EntityGroup(_testData.EntityGroup1Id, null, MwpTestData.EntityGroup1Name, _testData.EntityGroup1IsActive);
            var entityGroup2 = new EntityGroup(_testData.EntityGroup2Id, null, MwpTestData.EntityGroup2Name, _testData.EntityGroup2IsActive);
            await _entityGroupRepository.InsertAsync(entityGroup1);
            await _entityGroupRepository.InsertAsync(entityGroup2);

            // Act
            var result = await _entityGroupAppService.GetAsync(_testData.EntityGroup1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.EntityGroup1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EntityGroupCreateDto
            {
                Name = MwpTestData.EntityGroup2Name,
                IsActive = _testData.EntityGroup2IsActive
            };

            // Act
            var serviceResult = await _entityGroupAppService.CreateAsync(input);

            // Assert
            var result = await _entityGroupRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.EntityGroup2Name);
            result.IsActive.ShouldBe(_testData.EntityGroup2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var entityGroup1 = new EntityGroup(_testData.EntityGroup1Id, null, MwpTestData.EntityGroup1Name, _testData.EntityGroup1IsActive);
            await _entityGroupRepository.InsertAsync(entityGroup1);
            var input = new EntityGroupUpdateDto()
            {
                Name = MwpTestData.EntityGroup1Name,
                IsActive = _testData.EntityGroup1IsActive
            };

            // Act
            var serviceResult = await _entityGroupAppService.UpdateAsync(_testData.EntityGroup1Id, input);

            // Assert
            var result = await _entityGroupRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.EntityGroup1Name);
            result.IsActive.ShouldBe(_testData.EntityGroup1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var entityGroup2 = new EntityGroup(_testData.EntityGroup2Id, null, MwpTestData.EntityGroup2Name, _testData.EntityGroup2IsActive);
            await _entityGroupRepository.InsertAsync(entityGroup2);

            // Act
            await _entityGroupAppService.DeleteAsync(_testData.EntityGroup2Id);

            // Assert
            var result = await _entityGroupRepository.FindAsync(c => c.Id == _testData.EntityGroup2Id);

            result.ShouldBeNull();
        }
    }
}