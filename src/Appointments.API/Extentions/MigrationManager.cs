using Appointments.Infrastructure.Data;
using FluentMigrator.Runner;

namespace Appointments.API.Extentions;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        migrationService.ListMigrations();
        migrationService.MigrateUp();

		return host;
    }
}
