using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workshop.wwwapi.Data;
using workshop.wwwapi.Models;


[ApiController]
[Route("api/[controller]")]
public class AppointmentEndpoint : ControllerBase
{
    private readonly DatabaseContext _context;

    public AppointmentEndpoint(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAllAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                Booking = a.Booking,
                Doctor = new DoctorDTO { Id = a.Doctor.Id, FullName = a.Doctor.FullName },
                Patient = new PatientDTO { Id = a.Patient.Id, FullName = a.Patient.FullName }
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // GET: api/appointments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.Id == id)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                Booking = a.Booking,
                Doctor = new DoctorDTO { Id = a.Doctor.Id, FullName = a.Doctor.FullName },
                Patient = new PatientDTO { Id = a.Patient.Id, FullName = a.Patient.FullName }
            })
            .FirstOrDefaultAsync();

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    // GET: api/appointments/doctor/{doctorId}
    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctorId(int doctorId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                Booking = a.Booking,
                Doctor = new DoctorDTO { Id = a.Doctor.Id, FullName = a.Doctor.FullName },
                Patient = new PatientDTO { Id = a.Patient.Id, FullName = a.Patient.FullName }
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // GET: api/appointments/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByPatientId(int patientId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId)
            .Select(a => new AppointmentDto
            {
                Id = a.Id,
                Booking = a.Booking,
                Doctor = new DoctorDTO { Id = a.Doctor.Id, FullName = a.Doctor.FullName },
                Patient = new PatientDTO { Id = a.Patient.Id, FullName = a.Patient.FullName }
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // POST: api/appointments
    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto dto)
    {
        var appointment = new Appointment
        {
            Booking = dto.Booking,
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Load doctor/patient to return DTO properly
        appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == appointment.Id);

        var appointmentDto = new AppointmentDto
        {
            Id = appointment.Id,
            Booking = appointment.Booking,
            Doctor = new DoctorDTO { Id = appointment.Doctor.Id, FullName = appointment.Doctor.FullName },
            Patient = new PatientDTO { Id = appointment.Patient.Id, FullName = appointment.Patient.FullName }
        };

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointmentDto);
    }
}
