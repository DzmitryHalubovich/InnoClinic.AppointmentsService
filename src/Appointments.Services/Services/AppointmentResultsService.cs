using Appointments.Contracts.DTO.Result;
using Appointments.Domain.Entity;
using Appointments.Domain.Errors;
using Appointments.Domain.Interfaces;
using Appointments.Infrastructure.Repositories;
using Appointments.Services.Abstractions.Services;
using AutoMapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Appointments.Services.Services;

public class AppointmentResultsService : IAppointmentResultsService
{
    private readonly IAppointmentResultsRepository _appointmentResultsRepository;
    private readonly IMapper _mapper;
    private readonly DocumentsRepository _documentsRepository;

    public AppointmentResultsService(IAppointmentResultsRepository appointmentResultsRepository, IMapper mapper,
        DocumentsRepository documentsRepository)
    {
        _appointmentResultsRepository = appointmentResultsRepository;
        _mapper = mapper;
        _documentsRepository = documentsRepository;
    }

    public async Task<Guid> CreateAppointmentResultAsync(AppointmentResultCreateDTO newAppointmentResult)
    {
        var appointmentResult = _mapper.Map<AppointmentResult>(newAppointmentResult);

        var createdAppointmentResultId = await _appointmentResultsRepository.CreateAsync(appointmentResult);

        var fileName = createdAppointmentResultId;

        try
        {
            var pdfFile = GeneratePdfFile(newAppointmentResult);

            await _documentsRepository.UploadPdfFileAsync(pdfFile, fileName.ToString());

            return createdAppointmentResultId;
        }
        catch (HttpRequestException ex)
        {
            await _appointmentResultsRepository.DeleteAsync(createdAppointmentResultId);

            throw new HttpRequestException("Something went wrong during request to outer service.");
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
        var appointmentResultEntity = await _appointmentResultsRepository.GetByIdAsync(id);

        if (appointmentResultEntity is null)
        {
            throw new NotFoundException($"Result with id: {id} was not found in the database.");
        }

        var backUpResult = appointmentResultEntity;

        _mapper.Map(updatedAppointmentResult, appointmentResultEntity);

        await _appointmentResultsRepository.UpdateAsync(appointmentResultEntity);

        var fileName = appointmentResultEntity.Id;

        try
        {
            var pdfFile = GeneratePdfFile(updatedAppointmentResult);

            await _documentsRepository.DeletePdfFileAsync(fileName.ToString());

            await _documentsRepository.UploadPdfFileAsync(pdfFile, fileName.ToString());
        }
        catch (HttpRequestException)
        {
            await _appointmentResultsRepository.UpdateAsync(backUpResult);

            throw new HttpRequestException("Something went wrong during request to outer service.");
        }
    }


    private byte[] GeneratePdfFile<T>(T result) where T : AppointmentResultBaseDTO
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var patientFullName = string.Join(" ",
    new[] { result.PatientFirstName, result.PatientMiddleName, result.PatientLastName }.Where(x => !string.IsNullOrEmpty(x)));

        var doctorFullName = string.Join(" ",
            new[] { result.DoctorFirstName, result.DoctorMiddleName, result.DoctorLastName }.Where(x => !string.IsNullOrEmpty(x)));

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
                    .Text("Результаты посещения врача")
                    .Bold()
                    .FontSize(24)
                    .FontColor(Colors.Black);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(4);

                        x.Item().Text($"Дата приема:").FontSize(18).SemiBold();
                        x.Item().Text(result.AppointmentDate.ToString("dd/MM/yyyy HH:mm")).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Имя пациента: ").FontSize(18).SemiBold();
                        x.Item().Text(patientFullName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Дата рождения: ").FontSize(18).SemiBold();
                        x.Item().Text(result.PatientBirthDate.ToShortDateString()).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Имя доктора: ").FontSize(18).SemiBold();
                        x.Item().Text(doctorFullName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Специализация: ").FontSize(18).SemiBold();
                        x.Item().Text(result.DoctorSpecialization).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Услуга: ").FontSize(18).SemiBold();
                        x.Item().Text(result.ServiceName).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Жалобы пациента: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Complaints).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Заключение доктора: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Conclusion).FontColor(Colors.Blue.Darken4);

                        x.Item().PaddingTop(10f).Text("Рекомендации доктора: ").FontSize(18).SemiBold();
                        x.Item().Text(result.Recommendations).FontColor(Colors.Blue.Darken4);
                    });

                page.Footer().Text("Желаем крепокого здоровья! Приходите к нам ещё!").AlignCenter().FontSize(12);
            });
        }).GeneratePdf();

        return pdfFile;
    }
}
