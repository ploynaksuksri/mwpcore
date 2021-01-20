using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mwp.Migrations
{
    public class MwpTemporalMigration : Migration
    {
        protected List<string> TablesToUpdate = new List<string>();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in TablesToUpdate)
            {
                var alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] ADD SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START HIDDEN
                CONSTRAINT DF_{table}_SysStart DEFAULT GETDATE(), SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END HIDDEN
                CONSTRAINT DF_{table}_SysEnd DEFAULT CONVERT(datetime2 (0), '9999-12-31 23:59:59'),
                PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)";
                migrationBuilder.Sql(alterStatement);
                alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [{MwpConsts.DbSchema}].{table}_History));";
                migrationBuilder.Sql(alterStatement);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region OPTION 1 - Down() method

            foreach (var table in TablesToUpdate)
            {
                var alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] SET (SYSTEM_VERSIONING = OFF);";
                migrationBuilder.Sql(alterStatement);
                alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] DROP PERIOD FOR SYSTEM_TIME";
                migrationBuilder.Sql(alterStatement);
                alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] DROP DF_{table}_SysStart, DF_{table}_SysEnd";
                migrationBuilder.Sql(alterStatement);
                alterStatement = $@"ALTER TABLE [{MwpConsts.DbSchema}].[{table}] DROP COLUMN SysStartTime, COLUMN SysEndTime";
                migrationBuilder.Sql(alterStatement);
                alterStatement = $@"DROP TABLE [{MwpConsts.DbSchema}].[{table}]_History";
                migrationBuilder.Sql(alterStatement);
            }

            #endregion

            #region OPTION 2 - Rollback script

            // ALTER TABLE [mwp].[Forms] SET (SYSTEM_VERSIONING = OFF);
            // ALTER TABLE [mwp].[Forms] DROP PERIOD FOR SYSTEM_TIME
            // ALTER TABLE [mwp].[Forms] DROP DF_Forms_SysStart, DF_Forms_SysEnd
            // ALTER TABLE [mwp].[Forms] DROP COLUMN SysStartTime, COLUMN SysEndTime
            // DROP TABLE [mwp].[Forms_History]

            #endregion
        }
    }
}