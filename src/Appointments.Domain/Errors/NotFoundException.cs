namespace Appointments.Domain.Errors;

public class NotFoundException : Exception
{
    public NotFoundException(string error) : base(error) { }
}
