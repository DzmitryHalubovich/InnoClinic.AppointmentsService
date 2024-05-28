using Appointments.Contracts;
using Appointments.Contracts.DTO;
using Appointments.Domain.Entity;
using Appointments.Domain.Errors;
using Appointments.Domain.Interfaces;
using Appointments.Services.Abstraction;

namespace Appointments.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAppointmentsRepository _appointmentsRepository;

    public AppointmentsService(IAppointmentsRepository appointmentsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
    }

    public async Task<Guid> CreateAppointmentAsync(AppointmentCreateDTO newAppointment)
    {
        var appointment = new Appointment()
        {
            PatientId = newAppointment.PatientId,
            DoctorId = newAppointment.DoctorId,
            ServiceId = newAppointment.ServiceId,
            AppointmentDate = newAppointment.AppointmentDate,
            TimeSlot = TimeSpan.FromMinutes(newAppointment.TimeSlot),
            IsApproved = false
        };

        var appointmentGuid = await _appointmentsRepository.CreateAsync(appointment);

        return appointmentGuid;
    }

    public async Task<AppointmentResponseDTO> GetAppointmentByIdAsync(Guid id)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        var appointmentResponseDTO = new AppointmentResponseDTO()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            ServiceId = appointment.ServiceId,
            AppointmentDate = appointment.AppointmentDate,
            TimeSlot = appointment.TimeSlot,
            IsApproved = appointment.IsApproved
        };

        return appointmentResponseDTO;
    }

    public async Task<IEnumerable<AppointmentResponseDTO>> GetAllAppointmentsAsync(QueryParameters queryParameters)
    {
        var appointments = await _appointmentsRepository.GetAllAsync(queryParameters);

        if (!appointments.Any())
        {
            throw new NotFoundException("There are not any appointments in the database.");
        }

        var mappedAppointments = appointments.Select(x => new AppointmentResponseDTO()
        {
            Id = x.Id,
            PatientId = x.PatientId,
            DoctorId = x.DoctorId,
            ServiceId = x.ServiceId,
            AppointmentDate = x.AppointmentDate,
            IsApproved = x.IsApproved,
            TimeSlot = x.TimeSlot
        });

        return mappedAppointments;
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

    public async Task UpdateAppointmentAsync(Guid id, AppointmentUpdateDTO updatedAppointment)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        appointment.DoctorId = updatedAppointment.DoctorId;
        appointment.ServiceId = updatedAppointment.ServiceId;
        appointment.AppointmentDate = updatedAppointment.AppointmentDate;
        appointment.TimeSlot = TimeSpan.FromMinutes(updatedAppointment.TimeSlot);

        await _appointmentsRepository.UpdateAsync(appointment);
    }
}
