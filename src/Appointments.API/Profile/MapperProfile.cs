using Appointments.Contracts.DTO;
using Appointments.Domain.Entity;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<AppointmentCreateDTO, Appointment>();

        CreateMap<AppointmentUpdateDTO, Appointment>();

        CreateMap<Appointment, AppointmentResponseDTO>();
    }
}
