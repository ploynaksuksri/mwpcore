using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Mwp.EntityFrameworkCore
{
    public static class MwpDbContextDatabaseExtensions
    {
        public static DbCommand CreateQueryCommand(
            this DatabaseFacade database,
            string commandText,
            Dictionary<string, object> parameters = null)
        {
            var command = database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            command.Transaction = database.CurrentTransaction?.GetDbTransaction();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CreateParameter(command, parameter.Key, parameter.Value));
                }
            }

            return command;
        }


        private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
        {
            var aParam = cmd.CreateParameter();
            aParam.ParameterName = name;
            aParam.Value = value;
            return aParam;
        }
    }
}