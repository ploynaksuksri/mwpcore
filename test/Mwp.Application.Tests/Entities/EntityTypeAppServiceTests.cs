using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Entities
{
    public class EntityTypeAppServiceTests : MwpApplicationTestBase
    {
        readonly IEntityTypeAppService _entityTypeAppService;
        readonly IRepository<EntityType, Guid> _entityTypeRepository;
        readonly MwpTestData _testData;

        public EntityTypeAppServiceTests()
        {
            _entityTypeAppService = GetRequiredService<IEntityTypeAppService>();
            _entityTypeRepository = GetRequiredService<IRepository<EntityType, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var entityType1 = new EntityType(_testData.EntityType1Id, null, MwpTestData.EntityType1Name, _testData.EntityType1IsActive);
            var entityType2 = new EntityType(_testData.EntityType2Id, null, MwpTestData.EntityType2Name, _testData.EntityType2IsActive);
            await _entityTypeRepository.InsertAsync(entityType1);
            await _entityTypeRepository.InsertAsync(entityType2);

            // Act
            var result = await _entityTypeAppService.GetListAsync(new GetEntityTypesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.EntityType1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.EntityType2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var entityType1 = new EntityType(_testData.EntityType1Id, null, MwpTestData.EntityType1Name, _testData.EntityType1IsActive);
            var entityType2 = new EntityType(_testData.EntityType2Id, null, MwpTestData.EntityType2Name, _testData.EntityType2IsActive);
            await _entityTypeRepository.InsertAsync(entityType1);
            await _entityTypeRepository.InsertAsync(entityType2);

            // Act
            var result = await _entityTypeAppService.GetAsync(_testData.EntityType1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.EntityType1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EntityTypeCreateDto
            {
                Name = MwpTestData.EntityType2Name,
                IsActive = _testData.EntityType2IsActive
            };

            // Act
            var serviceResult = await _entityTypeAppService.CreateAsync(input);

            // Assert
            var result = await _entityTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.EntityType2Name);
            result.IsActive.ShouldBe(_testData.EntityType2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var entityType1 = new EntityType(_testData.EntityType1Id, null, MwpTestData.EntityType1Name, _testData.EntityType1IsActive);
            await _entityTypeRepository.InsertAsync(entityType1);
            var input = new EntityTypeUpdateDto()
            {
                Name = MwpTestData.EntityType1Name,
                IsActive = _testData.EntityType1IsActive
            };

            // Act
            var serviceResult = await _entityTypeAppService.UpdateAsync(_testData.EntityType1Id, input);

            // Assert
            var result = await _entityTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.EntityType1Name);
            result.IsActive.ShouldBe(_testData.EntityType1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var entityType2 = new EntityType(_testData.EntityType2Id, null, MwpTestData.EntityType2Name, _testData.EntityType2IsActive);
            await _entityTypeRepository.InsertAsync(entityType2);

            // Act
            await _entityTypeAppService.DeleteAsync(_testData.EntityType2Id);

            // Assert
            var result = await _entityTypeRepository.FindAsync(c => c.Id == _testData.EntityType2Id);

            result.ShouldBeNull();
        }
    }
}