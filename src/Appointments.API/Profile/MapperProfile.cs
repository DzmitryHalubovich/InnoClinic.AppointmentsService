using Appointments.Contracts.DTO.Appointment;
using Appointments.Contracts.DTO.Result;
using Appointments.Domain.Entity;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<AppointmentCreateDTO, Appointment>();

        CreateMap<AppointmentUpdateDTO, Appointment>();

        CreateMap<Appointment, AppointmentResponseDTO>();

        CreateMap<AppointmentResultCreateDTO, AppointmentResult>();

        CreateMap<AppointmentResult, AppointmentResultResponseDTO>();

        CreateMap<AppointmentResultUpdateDTO, AppointmentResult>();
    }
}
