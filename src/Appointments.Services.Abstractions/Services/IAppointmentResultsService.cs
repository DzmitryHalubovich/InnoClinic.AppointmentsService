using Appointments.Contracts.DTO.Result;

namespace Appointments.Services.Abstractions.Services;

public interface IAppointmentResultsService
{
    public Task<AppointmentResultResponseDTO> GetAppintmentResultByIdAsync(Guid id);

    public Task<Guid> CreateAppointmentResultAsync(AppointmentResultCreateDTO newAppointmentResult);

    public Task UpdateAppointmentResultAsync(Guid id, AppointmentResultUpdateDTO updatedAppointmentResult);
}
