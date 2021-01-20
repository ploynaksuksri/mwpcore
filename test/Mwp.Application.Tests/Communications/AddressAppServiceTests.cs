using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.DataSeed;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Communications
{
    public class AddressAppServiceTests : MwpApplicationTestBase
    {
        readonly IAddressAppService _addressAppService;
        readonly IRepository<Address, Guid> _addressRepository;
        readonly MwpTestData _testData;

        public AddressAppServiceTests()
        {
            _addressAppService = GetRequiredService<IAddressAppService>();
            _addressRepository = GetRequiredService<IRepository<Address, Guid>>();
            _testData = GetRequiredService<MwpTestData>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            var address1 = new Address(_testData.Address1Id, null, MwpTestData.Address1Name, _testData.Address1IsActive);
            var address2 = new Address(_testData.Address2Id, null, MwpTestData.Address2Name, _testData.Address2IsActive);
            await _addressRepository.InsertAsync(address1);
            await _addressRepository.InsertAsync(address2);

            // Act
            var result = await _addressAppService.GetListAsync(new GetAddressesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == _testData.Address1Id).ShouldBe(true);
            result.Items.Any(x => x.Id == _testData.Address2Id).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            var address1 = new Address(_testData.Address1Id, null, MwpTestData.Address1Name, _testData.Address1IsActive);
            var address2 = new Address(_testData.Address2Id, null, MwpTestData.Address2Name, _testData.Address2IsActive);
            await _addressRepository.InsertAsync(address1);
            await _addressRepository.InsertAsync(address2);

            // Act
            var result = await _addressAppService.GetAsync(_testData.Address1Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(_testData.Address1Id);
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new AddressCreateDto
            {
                Name = MwpTestData.Address2Name,
                IsActive = _testData.Address2IsActive
            };

            // Act
            var serviceResult = await _addressAppService.CreateAsync(input);

            // Assert
            var result = await _addressRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Address2Name);
            result.IsActive.ShouldBe(_testData.Address2IsActive);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var address1 = new Address(_testData.Address1Id, null, MwpTestData.Address1Name, _testData.Address1IsActive);
            await _addressRepository.InsertAsync(address1);
            var input = new AddressUpdateDto()
            {
                Name = MwpTestData.Address1Name,
                IsActive = _testData.Address1IsActive
            };

            // Act
            var serviceResult = await _addressAppService.UpdateAsync(_testData.Address1Id, input);

            // Assert
            var result = await _addressRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe(MwpTestData.Address1Name);
            result.IsActive.ShouldBe(_testData.Address1IsActive);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            var address2 = new Address(_testData.Address2Id, null, MwpTestData.Address2Name, _testData.Address2IsActive);
            await _addressRepository.InsertAsync(address2);

            // Act
            await _addressAppService.DeleteAsync(_testData.Address2Id);

            // Assert
            var result = await _addressRepository.FindAsync(c => c.Id == _testData.Address2Id);

            result.ShouldBeNull();
        }
    }
}