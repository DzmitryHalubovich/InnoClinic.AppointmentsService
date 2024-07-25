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
            .WithColumn("service_id").AsInt32().NotNullable()
            .WithColumn("service_name").AsString(100).NotNullable()
            .WithColumn("specialization_id").AsInt32().NotNullable()
            .WithColumn("specialization_name").AsString(100).NotNullable()
            .WithColumn("patient_id").AsGuid().NotNullable()
            .WithColumn("doctor_id").AsGuid().NotNullable()
            .WithColumn("office_id").AsString(24).NotNullable()
            .WithColumn("office_address").AsString(100).NotNullable()
            .WithColumn("appointment_date").AsDate().NotNullable()
            .WithColumn("patient_full_name").AsString(100).NotNullable()
            .WithColumn("doctor_full_name").AsString(100).NotNullable()
            .WithColumn("patient_email").AsString(100).NotNullable()
            .WithColumn("is_approved").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("is_notification_sent").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("appointment_results")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewSequentialId)
            .WithColumn("appointment_id").AsGuid().NotNullable().ForeignKey("appointments", "id")
            .WithColumn("complaints").AsString(200).NotNullable()
            .WithColumn("conclusion").AsString(500).NotNullable()
            .WithColumn("recommendations").AsString(500).NotNullable();
    }
}
