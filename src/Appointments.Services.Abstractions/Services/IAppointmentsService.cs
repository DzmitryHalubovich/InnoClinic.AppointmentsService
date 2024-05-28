using Appointments.Contracts;
using Appointments.Contracts.DTO;
namespace Appointments.Services.Abstraction;

public interface IAppointmentsService
{
    public Task<IEnumerable<AppointmentResponseDTO>> GetAllAppointmentsAsync(QueryParameters queryParameters);

    public Task<AppointmentResponseDTO> GetAppointmentByIdAsync(Guid id);

    public Task<Guid> CreateAppointmentAsync(AppointmentCreateDTO newAppointment);

    public Task UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO updatedAppointment);

    public Task ApproveAppointmentAsync(Guid id);

    public Task DeleteAppointmentAsync(Guid id);
}
