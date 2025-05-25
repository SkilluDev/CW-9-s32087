using APBD9.Models;

namespace APBD9.DTOs;

public class PrescriptionGetDto
{
    public int IdPrescription { get; set; }
    public PatientGetDto Patient { get; set; }
    public DoctorGetDto Doctor { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    
    public ICollection<PrescriptionMedicamentGetDto> Prescriptions { get; set; }
}