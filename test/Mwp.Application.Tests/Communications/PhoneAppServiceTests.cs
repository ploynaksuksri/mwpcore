using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Communications
{
    public class PhoneAppServiceTests : MwpApplicationTestBase
    {
        readonly IPhoneAppService _phoneAppService;
        readonly IRepository<Phone, Guid> _phoneRepository;
        readonly MwpTestData _testData;

        public PhoneAppServiceTests()
        {
            _phoneAppService = GetRequiredService<IPhoneAppService>();
            _phoneRepository = GetRequiredService<IRepository<Phone, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var phone1 = new Phone(_testData.Phone1Id, null, MwpTestData.Phone1Name, _testData.Phone1IsActive);
            var phone2 = new Phone(_testData.Phone2Id, null, MwpTestData.Phone2Name, _testData.Phone2IsActive);
            await _phoneRepository.InsertAsync(phone1);
            await _phoneRepository.InsertAsync(phone2);

            // Act
            var result = await _phoneAppService.GetListAsync(new GetPhonesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Phone1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Phone2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var phone1 = new Phone(_testData.Phone1Id, null, MwpTestData.Phone1Name, _testData.Phone1IsActive);
            var phone2 = new Phone(_testData.Phone2Id, null, MwpTestData.Phone2Name, _testData.Phone2IsActive);
            await _phoneRepository.InsertAsync(phone1);
            await _phoneRepository.InsertAsync(phone2);

            // Act
            var result = await _phoneAppService.GetAsync(_testData.Phone1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Phone1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PhoneCreateDto
            {
                Name = MwpTestData.Phone2Name,
                IsActive = _testData.Phone2IsActive
            };

            // Act
            var serviceResult = await _phoneAppService.CreateAsync(input);

            // Assert
            var result = await _phoneRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Phone2Name);
            result.IsActive.ShouldBe(_testData.Phone2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var phone1 = new Phone(_testData.Phone1Id, null, MwpTestData.Phone1Name, _testData.Phone1IsActive);
            await _phoneRepository.InsertAsync(phone1);
            var input = new PhoneUpdateDto()
            {
                Name = MwpTestData.Phone1Name,
                IsActive = _testData.Phone1IsActive
            };

            // Act
            var serviceResult = await _phoneAppService.UpdateAsync(_testData.Phone1Id, input);

            // Assert
            var result = await _phoneRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Phone1Name);
            result.IsActive.ShouldBe(_testData.Phone1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var phone1 = new Phone(_testData.Phone1Id, null, MwpTestData.Phone1Name, _testData.Phone1IsActive);
            await _phoneRepository.InsertAsync(phone1);

            // Act
            await _phoneAppService.DeleteAsync(_testData.Phone2Id);

            // Assert
            var result = await _phoneRepository.FindAsync(c => c.Id == _testData.Phone2Id);

            result.ShouldBeNull();
        }
    }
}