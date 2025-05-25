using APBD9.Models;

namespace APBD9.DTOs;

public class PrescriptionGetDto
{
    public int IdPrescription { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DueDate { get; set; }
    
    public ICollection<PrescriptionMedicamentGetDto> Medicaments { get; set; }
}