using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mwp.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Mwp.Data
{
    public sealed class MwpDbMigratoinServiceTests : MwpDomainTestBase
    {
        private readonly IMwpRollbackHelper _mwpRollbackHelper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMwpDbSchemaMigrator _mwpDbSchemaMigrator;

        public MwpDbMigratoinServiceTests()
        {
            _mwpRollbackHelper = GetRequiredService<IMwpRollbackHelper>();
            _serviceProvider = GetRequiredService<IServiceProvider>();
            _mwpDbSchemaMigrator = GetRequiredService<IMwpDbSchemaMigrator>();
        }

        [Fact]
        public void Migration_Should_have_migrations()
        {
            var migrations = _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .GetMigrations();

            migrations.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Migratoin_Should_get_latest_migration()
        {
            var latestMigration = _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .GetMigrations()
                .LastOrDefault();

            latestMigration.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Rollback_Should_get_rollback_script_from_migration_name()
        {
            var latestMigration = _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .GetMigrations().LastOrDefault();

            var rollbackCommand = _mwpRollbackHelper.GetRollbackScript(latestMigration);
            rollbackCommand.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Rollback_Should_run_successfully()
        {
            // STEP 1 : Create Table
            await _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .ExecuteSqlRawAsync("CREATE TABLE Table1 ( ID INTEGER PRIMARY KEY, NAME NVARCHAR(10) )");

            // STEP 2 : Query
            var queryCommand = "SELECT * FROM Table1";
            var tableExists = _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .ExecuteSqlRawAsync(queryCommand);
            tableExists.Exception.ShouldBeNull();

            // STEP 3 : Rollback
            await _mwpDbSchemaMigrator.RollbackAsync("DROP TABLE Table1");

            // STEP 4 : Query again
            var tableNotExist = _serviceProvider
                .GetRequiredService<MwpMigrationsDbContext>()
                .Database
                .ExecuteSqlRawAsync(queryCommand);
            tableNotExist.Exception.ShouldNotBeNull();
        }
    }
}