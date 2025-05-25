using APBD9.DTOs;
using APBD9.Models;
using APBD9.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController(IDbService service):ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<PatientGetDto>> GetPatient(int id)
    {
        var patient = await service.GetPatientAsync(id);
        
        return Ok(patient);
    }
}