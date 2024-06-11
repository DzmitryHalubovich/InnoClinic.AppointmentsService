using Appointments.Contracts;
using Appointments.Domain.Entity;

namespace Appointments.Domain.Interfaces;

public interface IAppointmentsRepository
{
    public Task<IEnumerable<Appointment>> GetAllAsync(QueryParameters queryParameters);

    public Task<Appointment?> GetByIdAsync(Guid id);

    public Task<Guid> CreateAsync(Appointment appointment);

    public Task UpdateAsync(Appointment appointment);

    public Task ApproveAsync(Guid id);

    public Task DeleteAsync(Guid id);

    public Task DeleteAllForDeletedServiceAsync(int serviceId);
}
