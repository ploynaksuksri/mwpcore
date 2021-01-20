using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Communications
{
    public class EmailAppServiceTests : MwpApplicationTestBase
    {
        readonly IEmailAppService _emailAppService;
        readonly IRepository<Email, Guid> _emailRepository;
        readonly MwpTestData _testData;

        public EmailAppServiceTests()
        {
            _emailAppService = GetRequiredService<IEmailAppService>();
            _emailRepository = GetRequiredService<IRepository<Email, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var email1 = new Email(_testData.Email1Id, null, MwpTestData.Email1Name, _testData.Email1IsActive);
            var email2 = new Email(_testData.Email2Id, null, MwpTestData.Email2Name, _testData.Email2IsActive);
            await _emailRepository.InsertAsync(email1);
            await _emailRepository.InsertAsync(email2);

            // Act
            var result = await _emailAppService.GetListAsync(new GetEmailsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Email1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Email2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var email1 = new Email(_testData.Email1Id, null, MwpTestData.Email1Name, _testData.Email1IsActive);
            var email2 = new Email(_testData.Email2Id, null, MwpTestData.Email2Name, _testData.Email2IsActive);
            await _emailRepository.InsertAsync(email1);
            await _emailRepository.InsertAsync(email2);

            // Act
            var result = await _emailAppService.GetAsync(_testData.Email1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Email1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new EmailCreateDto
            {
                Name = MwpTestData.Email2Name,
                IsActive = _testData.Email2IsActive
            };

            // Act
            var serviceResult = await _emailAppService.CreateAsync(input);

            // Assert
            var result = await _emailRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Email2Name);
            result.IsActive.ShouldBe(_testData.Email2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var email1 = new Email(_testData.Email1Id, null, MwpTestData.Email1Name, _testData.Email1IsActive);
            await _emailRepository.InsertAsync(email1);
            var input = new EmailUpdateDto()
            {
                Name = MwpTestData.Email1Name,
                IsActive = _testData.Email1IsActive
            };

            // Act
            var serviceResult = await _emailAppService.UpdateAsync(_testData.Email1Id, input);

            // Assert
            var result = await _emailRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Email1Name);
            result.IsActive.ShouldBe(_testData.Email1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var email2 = new Email(_testData.Email2Id, null, MwpTestData.Email2Name, _testData.Email2IsActive);
            await _emailRepository.InsertAsync(email2);

            // Act
            await _emailAppService.DeleteAsync(_testData.Email2Id);

            // Assert
            var result = await _emailRepository.FindAsync(c => c.Id == _testData.Email2Id);

            result.ShouldBeNull();
        }
    }
}