using FluentMigrator;

namespace Appointments.Infrastructure.Data;

[Migration(202106280001)]
public class InitialTables_202106280001 : Migration
{
    public override void Down()
    {
        Delete.Table("innoclinic_appointments_hangfiredb");
        Delete.Table("innoclinic_appointmentsdb");
    }

    public override void Up()
    {
        Create.Table("appointments")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("serviceId").AsInt32().NotNullable()
            .WithColumn("serviceName").AsString(100).NotNullable()
            .WithColumn("specializationId").AsInt32().NotNullable()
            .WithColumn("specializationName").AsString(100).NotNullable()
            .WithColumn("patientId").AsGuid().NotNullable()
            .WithColumn("doctorId").AsGuid().NotNullable()
            .WithColumn("officeId").AsString(24).NotNullable()
            .WithColumn("officeAddress").AsString(100).NotNullable()
            .WithColumn("appointmentDate").AsDateTime().NotNullable()
            .WithColumn("patientFullName").AsString(100).NotNullable()
            .WithColumn("doctorFullName").AsString(100).NotNullable()
            .WithColumn("patientEmail").AsString(100).NotNullable()
            .WithColumn("isApproved").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("isNotificationSent").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("appointmentresults")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewSequentialId)
            .WithColumn("appointmentId").AsGuid().NotNullable().ForeignKey("appointments", "id")
            .WithColumn("patientBirthDate").AsDate().NotNullable()
            .WithColumn("complaints").AsString(200).NotNullable()
            .WithColumn("conclusion").AsString(500).NotNullable()
            .WithColumn("recommendations").AsString(500).NotNullable();
    }
}
