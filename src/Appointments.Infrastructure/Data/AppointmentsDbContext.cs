using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;
using System.Data;

namespace Appointments.Infrastructure.Data;

public class AppointmentsDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AppointmentsDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SQLConnection");
    }

    public IDbConnection CreateConnection() =>
        new NpgsqlConnection(_connectionString);

    public IDbConnection CreateDefailtConnection()
    {
        var defaultConnectionString = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = "postgres"
        }.ToString();

        return new NpgsqlConnection(defaultConnectionString);
    }

    public void EnsureDatabaseCreated(IEnumerable<string> databaseNames)
    {
        var query = $"SELECT 1 FROM pg_database WHERE datname = @name";

        using var connection = CreateDefailtConnection();

        foreach (var dbName in databaseNames)
        {
            var doesAppointmentsDbExist = connection.QueryFirstOrDefault<int>(query, new { name = dbName }) == 1;

            if (!doesAppointmentsDbExist)
            {
                Log.Warning($"Database {dbName} doesn't exist");

                var queryCreateDatabase = $"create database {dbName}";

                connection.Execute(queryCreateDatabase);
            }
            
            using var databaseConnection = CreateConnection();

/*            var collationQuery = @"CREATE COLLATION case_insensitive (
                                    provider = icu,      
                                    locale = 'und-u-ks-level2',
                                    deterministic = false    
                                   );";

            databaseConnection.Execute(collationQuery);*/

            var enambleGuidGeneration = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";";

            databaseConnection.Execute(enambleGuidGeneration);
        }
    }
}
