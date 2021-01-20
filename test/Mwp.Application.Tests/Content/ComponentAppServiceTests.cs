using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class ComponentAppServiceTests : MwpApplicationTestBase
    {
        readonly IComponentAppService _componentAppService;
        readonly IRepository<Component, Guid> _componentRepository;
        readonly MwpTestData _testData;

        public ComponentAppServiceTests()
        {
            _componentAppService = GetRequiredService<IComponentAppService>();
            _componentRepository = GetRequiredService<IRepository<Component, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var component1 = new Component(_testData.Component1Id, null, MwpTestData.Component1Name, _testData.Component1IsActive);
            var component2 = new Component(_testData.Component2Id, null, MwpTestData.Component2Name, _testData.Component2IsActive);
            await _componentRepository.InsertAsync(component1);
            await _componentRepository.InsertAsync(component2);

            // Act
            var result = await _componentAppService.GetListAsync(new GetComponentsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Component1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Component2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var component1 = new Component(_testData.Component1Id, null, MwpTestData.Component1Name, _testData.Component1IsActive);
            var component2 = new Component(_testData.Component2Id, null, MwpTestData.Component2Name, _testData.Component2IsActive);
            await _componentRepository.InsertAsync(component1);
            await _componentRepository.InsertAsync(component2);

            // Act
            var result = await _componentAppService.GetAsync(_testData.Component1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Component1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ComponentCreateDto
            {
                Name = MwpTestData.Template2Name,
                IsActive = _testData.Template2IsActive
            };

            // Act
            var serviceResult = await _componentAppService.CreateAsync(input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Template2Name);
            result.IsActive.ShouldBe(_testData.Template2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var component1 = new Component(_testData.Component1Id, null, MwpTestData.Component1Name, _testData.Component1IsActive);
            await _componentRepository.InsertAsync(component1);
            var input = new ComponentUpdateDto()
            {
                Name = MwpTestData.Template1Name,
                IsActive = _testData.Template1IsActive
            };

            // Act
            var serviceResult = await _componentAppService.UpdateAsync(_testData.Component1Id, input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Template1Name);
            result.IsActive.ShouldBe(_testData.Template1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var component2 = new Component(_testData.Component2Id, null, MwpTestData.Component2Name, _testData.Component2IsActive);
            await _componentRepository.InsertAsync(component2);

            // Act
            await _componentAppService.DeleteAsync(_testData.Component2Id);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == _testData.Component2Id);

            result.ShouldBeNull();
        }
    }
}