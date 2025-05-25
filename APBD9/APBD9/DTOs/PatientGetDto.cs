using System.ComponentModel.DataAnnotations;
using APBD9.Models;

namespace APBD9.DTOs;

public class PatientGetDto
{
    public int IdPatient { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public DateTime Birthdate { get; set; }

    public ICollection<Prescription>? Prescriptions { get; set; }
}