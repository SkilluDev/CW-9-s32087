using APBD9.DTOs;
using APBD9.Exceptions;
using APBD9.Models;
using APBD9.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionsController(IDbService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionPostDto prescriptionData)
    {
        try
        {
            var prescription = await service.CreatePrescriptionAsync(prescriptionData);
            return CreatedAtAction(nameof(AddPrescription), new { id = prescription.Id }, prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

}

