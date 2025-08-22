using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshop.wwwapi.Data;
using workshop.wwwapi.Models;


[ApiController]
[Route("api/[controller]")]
public class PatientsEndpoint : ControllerBase
{
    private readonly DatabaseContext _context;

    public PatientsEndpoint (DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
    {
        var patients = await _context.Patients
            .Select(p => new PatientDTO
            {
                Id = p.Id,
                FullName = p.FullName,
            })
            .ToListAsync();

        return Ok(patients);
    }

    // GET: api/patients/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDTO>> GetPatientById(int id)
    {
        var patient = await _context.Patients
            .Where(p => p.Id == id)
            .Select(p => new PatientDTO
            {
                Id = p.Id,
                FullName = p.FullName,
            })
            .FirstOrDefaultAsync();

        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    // POST: api/patients
    [HttpPost]
    public async Task<ActionResult<PatientDTO>> CreatePatient(CreatePatientDTO dto)
    {
        var patient = new Patient
        {
            FullName = dto.FullName,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        var patientDto = new PatientDTO
        {
            Id = patient.Id,
            FullName = patient.FullName,
        };

        return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patientDto);
    }
}
