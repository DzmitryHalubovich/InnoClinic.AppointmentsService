using Appointments.Contracts;
using Appointments.Contracts.DTO.Appointment;
using Appointments.Domain.Entity;
using Appointments.Domain.Errors;
using Appointments.Domain.Interfaces;
using Appointments.Services.Abstraction;
using AutoMapper;

namespace Appointments.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;

    public AppointmentsService(IAppointmentsRepository appointmentsRepository, IMapper mapper)
    {
        _appointmentsRepository = appointmentsRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentResponseDTO> GetAppointmentByIdAsync(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        var appointmentResponseDTO = _mapper.Map<AppointmentResponseDTO>(appointment);

        return appointmentResponseDTO;
    }

    public async Task<IEnumerable<AppointmentResponseDTO>> GetAllAppointmentsAsync(QueryParameters queryParameters)
    {
        var appointments = await _appointmentsRepository.GetAllAsync(queryParameters);

        if (!appointments.Any())
        {
            throw new NotFoundException("There are not any appointments in the database.");
        }

        var mappedAppointments = _mapper.Map<List<AppointmentResponseDTO>>(appointments);

        return mappedAppointments;
    }

    public async Task<Guid> CreateAppointmentAsync(AppointmentCreateDTO newAppointment)
    {
        var appointment = _mapper.Map<Appointment>(newAppointment);

        var appointmentGuid = await _appointmentsRepository.CreateAsync(appointment);

        return appointmentGuid;
    }

    public async Task UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO updatedAppointment)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        _mapper.Map(updatedAppointment, appointment);

        await _appointmentsRepository.UpdateAsync(appointment);
    }

    public async Task DeleteAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        await _appointmentsRepository.DeleteAsync(appointment.Id);
    }

    public async Task ApproveAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        await _appointmentsRepository.ApproveAsync(appointment.Id);
    }

    public async Task DeleteEveryAppointmentForDeletedServiceAsync(int serviceId)
    {
        await _appointmentsRepository.DeleteAllForDeletedServiceAsync(serviceId);
    }
}
