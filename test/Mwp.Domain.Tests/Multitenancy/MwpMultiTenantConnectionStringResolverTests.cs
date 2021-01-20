using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mwp.Configuration;
using Shouldly;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Saas.Tenants;
using Xunit;

namespace Mwp.MultiTenancy
{
    public class MwpMultiTenantConnectionStringResolverTests : MwpDomainTestBase
    {
        protected readonly Tenant _tenant1;
        protected readonly Tenant _tenant3;

        private readonly IConnectionStringResolver _connectionResolver;
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private IStringEncryptionService _encryptionService;

        private const string detaultConnectionString = "default-value";
        private const string detaultDb1ConnectionString = "db1-default-value";

        public MwpMultiTenantConnectionStringResolverTests()
        {
            _connectionResolver = GetRequiredService<IConnectionStringResolver>();
            _connectionResolver.ShouldBeOfType<MwpMultiTenantConnectionStringResolver>();

            _currentTenant = GetRequiredService<ICurrentTenant>();
            _tenantRepository = GetRequiredService<ITenantRepository>();

            _tenant1 = _tenantRepository.FindByNameAsync("T1").Result;
            _tenant3 = _tenantRepository.FindByNameAsync("T3").Result;
        }

        protected override void BeforeAddApplication(IServiceCollection services)
        {
            var configuration = ConfigurationUtils.BuildConfiguration();

            _encryptionService = new StringEncryptionService(Options.Create(
                new AbpStringEncryptionOptions
                {
                    Keysize = 256,
                    DefaultPassPhrase = configuration["StringEncryption:DefaultPassPhrase"],
                    InitVectorBytes = Encoding.ASCII.GetBytes(configuration["StringEncryption:InitVectorBytes"]),
                    DefaultSalt = Encoding.ASCII.GetBytes(configuration["StringEncryption:DefaultSalt"])
                }));

            services.Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = _encryptionService.Encrypt(detaultConnectionString);
                options.ConnectionStrings["db1"] = _encryptionService.Encrypt(detaultDb1ConnectionString);
            });
        }

        [Fact]
        public void Default_ConnectionString_ShouldBeDecrypted()
        {
            _connectionResolver.Resolve().ShouldBe("default-value");
            _connectionResolver.Resolve("db1").ShouldBe("db1-default-value");
        }

        [Fact]
        public async Task Tenant_WithConnectionString_ShouldBeDecrypted()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                _tenant1.SetDefaultConnectionString(_encryptionService.Encrypt("tenant1-default-value"));
                _tenant1.SetConnectionString("db1", _encryptionService.Encrypt("tenant1-db1-value"));
                await _tenantRepository.UpdateAsync(_tenant1);

                using (_currentTenant.Change(_tenant1.Id))
                {
                    _connectionResolver.Resolve().ShouldBe("tenant1-default-value");
                    _connectionResolver.Resolve("db1").ShouldBe("tenant1-db1-value");
                }
            });
        }

        [Fact]
        public void Tenant_WithoutConnectionString_ShouldBeDecrypted()
        {
            using (_currentTenant.Change(_tenant3.Id))
            {
                _connectionResolver.Resolve().ShouldBe(detaultConnectionString);
                _connectionResolver.Resolve("db1").ShouldBe(detaultDb1ConnectionString);
            }
        }
    }
}