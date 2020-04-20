using Dapper;
using Marketplace.Shared;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public class SqlCommandsRepository : ICommandsRepository
    {
        private readonly SqlConnection connection;
        public SqlCommandsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IEnumerable<Command>> GetCommandsAsync()
        {
            const string sql = "SELECT * FROM dbo.Commands;";
            return await connection.QueryAsync<Command>(sql);
        }

        public async Task<int> AddCommandAsync(Command command)
        {
            const string sql = "INSERT INTO dbo.Commands (CommandName, CommandHelp, CommandText, Expires, ExpireTime, ExpireCommand, ExecuteOnBuyerJoinServer) " +
                "VALUES (@CommandName, @CommandHelp, @CommandText, @Expires, @ExpireTime, @ExpireCommand, @ExecuteOnBuyerJoinServer); SELECT SCOPE_IDENTITY();";
            return await connection.ExecuteScalarAsync<int>(sql, command);
        }

        public async Task UpdateCommandAsync(Command command)
        {
            const string sql = "UPDATE dbo.Commands SET CommandName = @CommandName, CommandHelp = @CommandHelp, CommandText = @CommandText, Expires = @Expires, " +
                "ExpireTime = @ExpireTime, ExpireCommand = @ExpireCommand, ExecuteOnBuyerJoinServer = @ExecuteOnBuyerJoinServer WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, command);
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
