using System.ComponentModel.DataAnnotations;

namespace APBD9.DTOs;

public class PrescriptionPostDto
{
    public DateTime Date { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public PatientGetDto Patient { get; set; }
    
    public DoctorGetDto Doctor { get; set; }
    
    [MaxLength(10)]
    public ICollection<PrescriptionMedicamentGetDto> Medicaments { get; set; }
}