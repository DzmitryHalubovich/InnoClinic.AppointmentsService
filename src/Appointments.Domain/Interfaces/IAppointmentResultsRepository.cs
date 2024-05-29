using Appointments.Domain.Entity;

namespace Appointments.Domain.Interfaces;

public interface IAppointmentResultsRepository
{
    public Task<AppointmentResult?> GetByIdAsync(Guid id);

    public Task<Guid> CreateAsync(AppointmentResult appointment);

    public Task UpdateAsync(AppointmentResult appointmentResult);
}
