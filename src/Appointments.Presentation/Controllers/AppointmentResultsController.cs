using Appointments.Contracts.DTO.Result;
using Appointments.Services.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Appointments.Presentation.Controllers;

[ApiController]
[Route("api/appointmentResults")]
public class AppointmentResultsController : ControllerBase
{
    private readonly IAppointmentResultsService _appointmentResultsService;

    public AppointmentResultsController(IAppointmentResultsService appointmentResultsService)
    {
        _appointmentResultsService = appointmentResultsService;
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = "GetResultById")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetResultById([FromRoute] Guid id)
    {
        var appointmentResult = await _appointmentResultsService.GetAppintmentResultByIdAsync(id);

        return Ok(appointmentResult);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAppointmentResult([FromBody] AppointmentResultCreateDTO newResult)
    {
        var createdResultId = await _appointmentResultsService.CreateAppointmentResultAsync(newResult);

        return CreatedAtAction("GetResultById", new { id = createdResultId }, createdResultId);
    }

    [HttpPut("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateResult([FromRoute] Guid id, [FromBody] AppointmentResultUpdateDTO updatedResult)
    {
        await _appointmentResultsService.UpdateAppointmentResultAsync(id, updatedResult);

        return NoContent();
    }
}
