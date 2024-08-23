using Appointments.Contracts.DTO.Result;
using Appointments.Domain.Entity;
using Appointments.Domain.Errors;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;
using Appointments.Services.Abstractions.Services;
using AutoMapper;
using InnoClinic.SharedModels.MQMessages.Appointments;
using MassTransit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Appointments.Services.Services;

public class AppointmentResultsService : IAppointmentResultsService
{
    private readonly IAppointmentResultsRepository _appointmentResultsRepository;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IMapper _mapper;
    private readonly DocumentsServiceHttpClient _documentsRepository;
    private readonly IPublishEndpoint _messagePublisher;

    public AppointmentResultsService(IAppointmentResultsRepository appointmentResultsRepository, 
        IMapper mapper,
        DocumentsServiceHttpClient documentsRepository, 
        IAppointmentsRepository appointmentsRepository,
        IPublishEndpoint messagePublisher)
    {
        _appointmentResultsRepository = appointmentResultsRepository;
        _mapper = mapper;
        _documentsRepository = documentsRepository;
        _appointmentsRepository = appointmentsRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<Guid> CreateAppointmentResultAsync(AppointmentResultCreateDTO newAppointmentResult)
    {
        var appointment = await _appointmentsRepository.GetByIdAsync(newAppointmentResult.AppointmentId);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id = {newAppointmentResult.AppointmentId} was not found");
        }

        var appointmentResult = _mapper.Map<AppointmentResult>(newAppointmentResult);

        var createdAppointmentResultId = await _appointmentResultsRepository.CreateAsync(appointmentResult);

        var fileName = createdAppointmentResultId;

        try
        {
            var pdfFile = GeneratePdfFile(appointment, appointmentResult);

            await _documentsRepository.UploadPdfFileAsync(pdfFile, fileName.ToString());

            await _messagePublisher.Publish<AppointmentResultCreatedMessage>(new()
            {
                AppointmentResultId = createdAppointmentResultId,
                PatientEmail = appointment.PatientEmail,
                PatientFullName = appointment.PatientFullName
            });

            return createdAppointmentResultId;
        }
        catch (HttpRequestException)
        {
            await _appointmentResultsRepository.DeleteAsync(createdAppointmentResultId);

            throw;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<AppointmentResultResponseDTO> GetAppintmentResultByIdAsync(Guid id)
    {
        var appointmentResult = await _appointmentResultsRepository.GetByIdAsync(id);

        if (appointmentResult is null)
        {
            throw new NotFoundException($"Result with id: {id} was not found in the database.");
        }

        var mappedResult = _mapper.Map<AppointmentResultResponseDTO>(appointmentResult);

        return mappedResult;
    }

    public async Task UpdateAppointmentResultAsync(Guid id, AppointmentResultUpdateDTO updatedAppointmentResult)
    {
        var appointmentResult = await _appointmentResultsRepository.GetByIdAsync(id);

        if (appointmentResult is null)
        {
            throw new NotFoundException($"Appointment result with id: {id} was not found in the database.");
        }

        var appointment = await _appointmentsRepository.GetByIdAsync(appointmentResult.AppointmentId);

        if (appointment is null)
        {
            throw new NotFoundException($"Appointment with id: {id} was not found in the database.");
        }

        var backUpResult = new AppointmentResult();

        _mapper.Map(appointmentResult, backUpResult);

        _mapper.Map(updatedAppointmentResult, appointmentResult);

        await _appointmentResultsRepository.UpdateAsync(appointmentResult);

        var fileName = appointmentResult.Id;

        try
        {
            var pdfFile = GeneratePdfFile(appointment, appointmentResult);

            await _documentsRepository.UploadPdfFileAsync(pdfFile, fileName.ToString());

            await _messagePublisher.Publish<AppointmentResultUpdatedMessage>(new ()
            {
                AppointmentResultId = appointmentResult.Id,
                PatientEmail = appointment.PatientEmail,
                PatientFullName = appointment.PatientFullName
            });
        }
        catch (HttpRequestException)
        {
            await _appointmentResultsRepository.UpdateAsync(backUpResult);

            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }


    private byte[] GeneratePdfFile(Appointment appointment, AppointmentResult result)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfFile = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(16).FontFamily(Fonts.Arial));

                page.Header()
                    .AlignCenter()
                    .Text("Doctor’s report")
                    .Bold()
                    .FontSize(24)
                    .FontColor(Colors.Black);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(4);

                        x.Item().Text($"Appointment date:").FontSize(18).SemiBold();
                        x.Item().Text(appointment.AppointmentDate.ToString("dd/MM/yyyy HH:mm")).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Patient's name: ").FontSize(18).SemiBold();
                        x.Item().Text(appointment.PatientFullName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Birth day: ").FontSize(18).SemiBold();
                        x.Item().Text(result.PatientBirthDate.ToShortDateString()).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Doctor's name: ").FontSize(18).SemiBold();
                        x.Item().Text(appointment.AppointmentDate).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Doctor's specialization: ").FontSize(18).SemiBold();
                        x.Item().Text(appointment.SpecializationName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Service: ").FontSize(18).SemiBold();
                        x.Item().Text(appointment.ServiceName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Complaints: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Complaints).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Conclusions: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Conclusion).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Recomendations: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Recommendations).FontColor(Colors.Blue.Darken4);
                    });

                page.Footer().Text("We wish you good health! Come visit us again!").AlignCenter().FontSize(12);
            });
        }).GeneratePdf();

        return pdfFile;
    }
}
