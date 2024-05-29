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

        CreateMap<AppointmentResultCreateDTO, AppointmentResult>();

        CreateMap<AppointmentResult, AppointmentResultResponseDTO>()
            .ForMember(dest => dest.AppointmentResponseDTO, opt => opt.MapFrom(src => src.Appointment));

        CreateMap<AppointmentResultUpdateDTO, AppointmentResult>();
    }
}
