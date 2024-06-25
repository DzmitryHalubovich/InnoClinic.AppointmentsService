namespace Appointments.Contracts;

public class QueryParameters
{
    public Guid? DoctorId { get; set; } = null;

    public int? ServiceId { get; set; } = null;

    public Guid? PatientId { get; set; } = null;

    public string? OfficeId { get; set; } = null;

    public bool? OnlyApproved { get; set; } = false;
}
