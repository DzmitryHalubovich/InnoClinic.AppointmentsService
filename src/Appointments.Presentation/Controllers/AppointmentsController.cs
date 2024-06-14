using Appointments.Contracts;
using Appointments.Contracts.DTO.Appointment;
using Appointments.Services.Abstraction;
using Microsoft.AspNetCore.Http;
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
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllAppointments([FromQuery] QueryParameters queryParameters)
    {
        var appointments = await _appointmentsService.GetAllAppointmentsAsync(queryParameters);

        return Ok(appointments);
    }

    [HttpGet("{id}", Name = "GetAppointmentById")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
    {
        var appointment = await _appointmentsService.GetAppointmentByIdAsync(id);

        return Ok(appointment);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDTO newAppointment)
    {
        var createdAppointmentGuid = await _appointmentsService.CreateAppointmentAsync(newAppointment);

        return CreatedAtRoute("GetAppointmentById", new { id = createdAppointmentGuid }, createdAppointmentGuid);
    }

    [HttpPut("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAppointment([FromRoute] Guid id, [FromBody] AppointmentUpdateDTO updatedAppointment)
    {
        await _appointmentsService.UpdateAppointmentAsync(id, updatedAppointment);

        return NoContent();
    }

    [HttpPatch("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveAppointment([FromRoute] Guid id)
    {
        await _appointmentsService.ApproveAppointmentAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
    {
        await _appointmentsService.DeleteAppointmentAsync(id);

        return NoContent();
    }
}
