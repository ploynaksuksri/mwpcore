using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Entities
{
    public class RelationshipTypeAppServiceTests : MwpApplicationTestBase
    {
        readonly IRelationshipTypeAppService _relationshipTypeAppService;
        readonly IRepository<RelationshipType, Guid> _relationshipTypeRepository;
        readonly MwpTestData _testData;

        public RelationshipTypeAppServiceTests()
        {
            _relationshipTypeAppService = GetRequiredService<IRelationshipTypeAppService>();
            _relationshipTypeRepository = GetRequiredService<IRepository<RelationshipType, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var relationshipType1 = new RelationshipType(_testData.RelationshipType1Id, null, MwpTestData.RelationshipType1Name, _testData.RelationshipType1IsActive);
            var relationshipType2 = new RelationshipType(_testData.RelationshipType2Id, null, MwpTestData.RelationshipType2Name, _testData.RelationshipType2IsActive);
            await _relationshipTypeRepository.InsertAsync(relationshipType1);
            await _relationshipTypeRepository.InsertAsync(relationshipType2);

            // Act
            var result = await _relationshipTypeAppService.GetListAsync(new GetRelationshipTypesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.RelationshipType1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.RelationshipType2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var relationshipType1 = new RelationshipType(_testData.RelationshipType1Id, null, MwpTestData.RelationshipType1Name, _testData.RelationshipType1IsActive);
            var relationshipType2 = new RelationshipType(_testData.RelationshipType2Id, null, MwpTestData.RelationshipType2Name, _testData.RelationshipType2IsActive);
            await _relationshipTypeRepository.InsertAsync(relationshipType1);
            await _relationshipTypeRepository.InsertAsync(relationshipType2);

            // Act
            var result = await _relationshipTypeAppService.GetAsync(_testData.RelationshipType1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.RelationshipType1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new RelationshipTypeCreateDto
            {
                Name = MwpTestData.RelationshipType2Name,
                IsActive = _testData.RelationshipType2IsActive
            };

            // Act
            var serviceResult = await _relationshipTypeAppService.CreateAsync(input);

            // Assert
            var result = await _relationshipTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.RelationshipType2Name);
            result.IsActive.ShouldBe(_testData.RelationshipType2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var relationshipType1 = new RelationshipType(_testData.RelationshipType1Id, null, MwpTestData.RelationshipType1Name, _testData.RelationshipType1IsActive);
            await _relationshipTypeRepository.InsertAsync(relationshipType1);
            var input = new RelationshipTypeUpdateDto()
            {
                Name = MwpTestData.RelationshipType1Name,
                IsActive = _testData.RelationshipType1IsActive
            };

            // Act
            var serviceResult = await _relationshipTypeAppService.UpdateAsync(_testData.RelationshipType1Id, input);

            // Assert
            var result = await _relationshipTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.RelationshipType1Name);
            result.IsActive.ShouldBe(_testData.RelationshipType1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var relationshipType2 = new RelationshipType(_testData.RelationshipType2Id, null, MwpTestData.RelationshipType2Name, _testData.RelationshipType2IsActive);
            await _relationshipTypeRepository.InsertAsync(relationshipType2);

            // Act
            await _relationshipTypeAppService.DeleteAsync(_testData.RelationshipType2Id);

            // Assert
            var result = await _relationshipTypeRepository.FindAsync(c => c.Id == _testData.RelationshipType2Id);

            result.ShouldBeNull();
        }
    }
}