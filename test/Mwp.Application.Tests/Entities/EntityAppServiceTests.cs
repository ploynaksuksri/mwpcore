using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Entities
{
    public class EntityAppServiceTests : MwpApplicationTestBase
    {
        public const string SkipForNow = "The test will be implemented later due to lack of data seed.";

        readonly IEntityAppService _entityAppService;
        readonly IRepository<Entity, Guid> _entityRepository;
        readonly MwpTestData _testData;

        public EntityAppServiceTests()
        {
            _entityAppService = GetRequiredService<IEntityAppService>();
            _entityRepository = GetRequiredService<IRepository<Entity, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetListAsync()
        {
            // Arrange
            var entity1 = new Entity(_testData.Entity1Id, null, MwpTestData.Entity1Name, _testData.EntityType1Id, _testData.TenantEx1Id, _testData.EntityGroup1Id, _testData.Entity1IsActive);
            var entity2 = new Entity(_testData.Entity2Id, null, MwpTestData.Entity2Name, _testData.EntityType2Id, _testData.TenantEx2Id, _testData.EntityGroup2Id, _testData.Entity2IsActive);
            await _entityRepository.InsertAsync(entity1);
            await _entityRepository.InsertAsync(entity2);

            // Act
            var result = await _entityAppService.GetListAsync(new GetEntitiesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Entity1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Entity2Id).ShouldBe(true);
        }

        [Fact(Skip = SkipForNow)]
        public async Task GetAsync()
        {
            // Arrange
            var entity1 = new Entity(_testData.Entity1Id, null, MwpTestData.Entity1Name, _testData.EntityType1Id, _testData.TenantEx1Id, _testData.EntityGroup1Id, _testData.Entity1IsActive);
            var entity2 = new Entity(_testData.Entity2Id, null, MwpTestData.Entity2Name, _testData.EntityType2Id, _testData.TenantEx2Id, _testData.EntityGroup2Id, _testData.Entity2IsActive);
            await _entityRepository.InsertAsync(entity1);
            await _entityRepository.InsertAsync(entity2);

            // Act
            var result = await _entityAppService.GetAsync(_testData.Entity1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Entity1Id);
        }

        [Fact(Skip = SkipForNow)]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EntityCreateDto
            {
                Name = MwpTestData.Entity2Name,
                IsActive = _testData.Entity2IsActive
            };

            // Act
            var serviceResult = await _entityAppService.CreateAsync(input);

            // Assert
            var result = await _entityRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Entity2Name);
            result.IsActive.ShouldBe(_testData.Entity2IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task UpdateAsync()
        {
            // Arrange
            var entity1 = new Entity(_testData.Entity1Id, null, MwpTestData.Entity1Name, _testData.EntityType1Id, _testData.TenantEx1Id, _testData.EntityGroup1Id, _testData.Entity1IsActive);
            await _entityRepository.InsertAsync(entity1);
            var input = new EntityUpdateDto()
            {
                Name = MwpTestData.Entity1Name,
                IsActive = _testData.Entity1IsActive
            };

            // Act
            var serviceResult = await _entityAppService.UpdateAsync(_testData.Entity1Id, input);

            // Assert
            var result = await _entityRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Entity1Name);
            result.IsActive.ShouldBe(_testData.Entity1IsActive);
        }

        [Fact(Skip = SkipForNow)]
        public async Task DeleteAsync()
        {
            // Arrange
            var entity2 = new Entity(_testData.Entity2Id, null, MwpTestData.Entity2Name, _testData.EntityType2Id, _testData.TenantEx2Id, _testData.EntityGroup2Id, _testData.Entity2IsActive);
            await _entityRepository.InsertAsync(entity2);

            // Act
            await _entityAppService.DeleteAsync(_testData.Entity2Id);

            // Assert
            var result = await _entityRepository.FindAsync(c => c.Id == _testData.Entity2Id);

            result.ShouldBeNull();
        }
    }
}