using Microsoft.Extensions.Configuration;
using Npgsql;
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
}
