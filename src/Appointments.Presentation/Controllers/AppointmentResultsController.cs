using Appointments.Contracts.DTO;
using Appointments.Services.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Appointments.Presentation.Controllers;

[ApiController]
[Route("api/results")]
public class AppointmentResultsController : ControllerBase
{
    private readonly IAppointmentResultsService _appointmentResultsService;

    public AppointmentResultsController(IAppointmentResultsService appointmentResultsService)
    {
        _appointmentResultsService = appointmentResultsService;
    }

    [HttpGet("{id}", Name = "GetResultById")]
    public async Task<IActionResult> GetResultById([FromRoute] Guid id)
    {
        var appointmentResult = await _appointmentResultsService.GetAppintmentResultByIdAsync(id);

        return Ok(appointmentResult);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointmentResult([FromBody] AppointmentResultCreateDTO newResult)
    {
        var createdResultId = await _appointmentResultsService.CreateAppointmentResultAsync(newResult);

        return CreatedAtAction("GetResultById", new { id = createdResultId }, createdResultId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResult([FromRoute] Guid id, [FromBody] AppointmentResultUpdateDTO updatedResult)
    {
        await _appointmentResultsService.UpdateAppointmentResultAsync(id, updatedResult);

        return NoContent();
    }

    [Route("{id}/download")]
    [HttpGet]
    public async Task<IActionResult> DownloadAppointmentResult([FromRoute] Guid id)
    {
        var appointmentResult = await _appointmentResultsService.GetAppintmentResultByIdAsync(id);

        var pdfBytes = _appointmentResultsService.GeneratePdfFile(appointmentResult);

        return File(pdfBytes, "application/pdf", "FileName.pdf");
    }
}
