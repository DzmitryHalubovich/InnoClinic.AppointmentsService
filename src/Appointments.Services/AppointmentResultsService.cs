using Appointments.Contracts.DTO;
using Appointments.Domain.Entity;
using Appointments.Domain.Errors;
using Appointments.Domain.Interfaces;
using Appointments.Services.Abstractions.Services;
using AutoMapper;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Appointments.Services;

public class AppointmentResultsService : IAppointmentResultsService
{
    private readonly IAppointmentResultsRepository _appointmentResultsRepository;
    private readonly IMapper _mapper;

    public AppointmentResultsService(IAppointmentResultsRepository appointmentResultsRepository, IMapper mapper)
    {
        _appointmentResultsRepository = appointmentResultsRepository;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAppointmentResultAsync(AppointmentResultCreateDTO newAppointmentResult)
    {
        var mappedResult = _mapper.Map<AppointmentResult>(newAppointmentResult);

        var createdAppointmentId = await _appointmentResultsRepository.CreateAsync(mappedResult);

        return createdAppointmentId;
    }

    public byte[] GeneratePdfFile(AppointmentResultResponseDTO appointmentResult)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 14, XFontStyleEx.Bold);

            gfx.DrawString($"Дата приема: {appointmentResult.AppointmentResponseDTO.AppointmentDate}", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"Жалобы: {appointmentResult.Complaints}", font, XBrushes.Black, new XRect(0, 40, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"Рекомендации: {appointmentResult.Complaints}", font, XBrushes.Black, new XRect(0, 80, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"Заключение врача: {appointmentResult.Conclusion}", font, XBrushes.Black, new XRect(0, 120, page.Width, page.Height), XStringFormats.TopLeft);

            document.Save(stream, false);

            return stream.ToArray();
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
            throw new NotFoundException($"Result with id: {id} was not found in the database.");
        }

        _mapper.Map(updatedAppointmentResult, appointmentResult);

        await _appointmentResultsRepository.UpdateAsync(appointmentResult);
    }
}
