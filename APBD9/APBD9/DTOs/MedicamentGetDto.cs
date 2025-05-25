using APBD9.Models;

namespace APBD9.DTOs;

public class MedicamentGetDto
{
    public int IdMedicament { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public string Type { get; set; }

}