using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Mwp.Tenants.Dtos;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Tenants
{
    public class TenantTypeAppServiceTests : MwpApplicationTestBase
    {
        readonly ITenantTypeAppService _tenantTypeAppService;
        readonly IRepository<TenantType, Guid> _tenantTypeRepository;
        readonly MwpTestData _testData;

        public TenantTypeAppServiceTests()
        {
            _tenantTypeAppService = GetRequiredService<ITenantTypeAppService>();
            _tenantTypeRepository = GetRequiredService<IRepository<TenantType, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var tenantType1 = new TenantType(_testData.TenantType1Id, null, MwpTestData.TenantType1Name, _testData.TenantType1IsActive);
            var tenantType2 = new TenantType(_testData.TenantType2Id, null, MwpTestData.TenantType2Name, _testData.TenantType2IsActive);
            await _tenantTypeRepository.InsertAsync(tenantType1);
            await _tenantTypeRepository.InsertAsync(tenantType2);

            // Act
            var result = await _tenantTypeAppService.GetListAsync(new GetTenantTypesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.TenantType1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.TenantType2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var tenantType1 = new TenantType(_testData.TenantType1Id, null, MwpTestData.TenantType1Name, _testData.TenantType1IsActive);
            await _tenantTypeRepository.InsertAsync(tenantType1);

            // Act
            var result = await _tenantTypeAppService.GetAsync(_testData.TenantType1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.TenantType1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TenantTypeCreateDto
            {
                Name = MwpTestData.TenantType2Name,
                IsActive = _testData.TenantType2IsActive
            };

            // Act
            var serviceResult = await _tenantTypeAppService.CreateAsync(input);

            // Assert
            var result = await _tenantTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.TenantType2Name);
            result.IsActive.ShouldBe(_testData.TenantType2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var tenantType1 = new TenantType(_testData.TenantType1Id, null, MwpTestData.TenantType1Name, !_testData.TenantType1IsActive);
            await _tenantTypeRepository.InsertAsync(tenantType1);
            var input = new TenantTypeUpdateDto
            {
                Name = MwpTestData.TenantType1Name,
                IsActive = _testData.TenantType1IsActive
            };

            // Act
            var serviceResult = await _tenantTypeAppService.UpdateAsync(_testData.TenantType1Id, input);

            // Assert
            var result = await _tenantTypeRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.TenantType1Name);
            result.IsActive.ShouldBe(_testData.TenantType1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var tenantType2 = new TenantType(_testData.TenantType2Id, null, MwpTestData.TenantType2Name, _testData.TenantType2IsActive);
            await _tenantTypeRepository.InsertAsync(tenantType2);

            // Act
            await _tenantTypeAppService.DeleteAsync(_testData.TenantType2Id);

            // Assert
            var result = await _tenantTypeRepository.FindAsync(c => c.Id == _testData.TenantType2Id);

            result.ShouldBeNull();
        }
    }
}