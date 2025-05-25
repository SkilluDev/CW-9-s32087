using APBD9.Models;

namespace APBD9.DTOs;

public class PrescriptionMedicamentGetDto
{
    public int IdMedicament { get; set; }
    
    public int? Dose { get; set; }
    
    public string? Description { get; set; }
    
    public MedicamentGetDto? Medicament { get; set; }
}