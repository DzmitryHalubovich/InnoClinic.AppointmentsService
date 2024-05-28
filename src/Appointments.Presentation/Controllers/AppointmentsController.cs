using Appointments.Contracts;
using Appointments.Contracts.DTO;
using Appointments.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Presentation.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentsService _appointmentsService;

    public AppointmentsController(IAppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAppointments([FromQuery] QueryParameters queryParameters)
    {
        var appointments = await _appointmentsService.GetAllAppointmentsAsync(queryParameters);

        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
    {
        var appointment = await _appointmentsService.GetAppointmentByIdAsync(id);

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllAppointments([FromBody] AppointmentCreateDTO newAppointment)
    {
        var createdAppointmentGuid = await _appointmentsService.CreateAppointmentAsync(newAppointment);

        return Ok(createdAppointmentGuid);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, [FromBody] AppointmentUpdateDTO updatedAppointment)
    {
        await _appointmentsService.UpdateAppointmentAsync(id, updatedAppointment);

        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> ApproveAppointment([FromRoute] Guid id)
    {
        await _appointmentsService.ApproveAppointmentAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
    {
        await _appointmentsService.DeleteAppointmentAsync(id);

        return NoContent();
    }
}
