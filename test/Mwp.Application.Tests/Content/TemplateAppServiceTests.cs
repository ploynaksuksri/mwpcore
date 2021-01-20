using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Content
{
    public class TemplateAppServiceTests : MwpApplicationTestBase
    {
        readonly ITemplateAppService _templateAppService;
        readonly IRepository<Template, Guid> _templateRepository;
        readonly MwpTestData _testData;

        public TemplateAppServiceTests()
        {
            _templateAppService = GetRequiredService<ITemplateAppService>();
            _templateRepository = GetRequiredService<IRepository<Template, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var template1 = new Template(_testData.Template1Id, null, MwpTestData.Template1Name, _testData.Template1IsActive);
            var template2 = new Template(_testData.Template2Id, null, MwpTestData.Template2Name, _testData.Template2IsActive);
            await _templateRepository.InsertAsync(template1);
            await _templateRepository.InsertAsync(template2);

            // Act
            var result = await _templateAppService.GetListAsync(new GetTemplatesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Template1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Template2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var template1 = new Template(_testData.Template1Id, null, MwpTestData.Template1Name, _testData.Template1IsActive);
            var template2 = new Template(_testData.Template2Id, null, MwpTestData.Template2Name, _testData.Template2IsActive);
            await _templateRepository.InsertAsync(template1);
            await _templateRepository.InsertAsync(template2);

            // Act
            var result = await _templateAppService.GetAsync(_testData.Template1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Template1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new TemplateCreateDto
            {
                Name = MwpTestData.Template2Name,
                IsActive = _testData.Template2IsActive
            };

            // Act
            var serviceResult = await _templateAppService.CreateAsync(input);

            // Assert
            var result = await _templateRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Template2Name);
            result.IsActive.ShouldBe(_testData.Template2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var template1 = new Template(_testData.Template1Id, null, MwpTestData.Template1Name, _testData.Template1IsActive);
            await _templateRepository.InsertAsync(template1);
            var input = new TemplateUpdateDto()
            {
                Name = MwpTestData.Template1Name,
                IsActive = _testData.Template1IsActive
            };

            // Act
            var serviceResult = await _templateAppService.UpdateAsync(_testData.Template1Id, input);

            // Assert
            var result = await _templateRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Template1Name);
            result.IsActive.ShouldBe(_testData.Template1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var template2 = new Template(_testData.Template2Id, null, MwpTestData.Template2Name, _testData.Template2IsActive);
            await _templateRepository.InsertAsync(template2);

            // Act
            await _templateAppService.DeleteAsync(_testData.Template2Id);

            // Assert
            var result = await _templateRepository.FindAsync(c => c.Id == _testData.Template2Id);

            result.ShouldBeNull();
        }
    }
}