using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Models;

[Table("Prescription")]
[PrimaryKey(nameof(IdPrescription),nameof(IdPatient),nameof(IdDoctor))]
public class Prescription
{
    [Column("IdPrescription")]
    public int IdPrescription { get; set; }
    
    public DateTime? Date { get; set; } = null!;
    
    public DateTime? DueDate { get; set; } = null!;
    
    [Column("IdPatient")]
    public int IdPatient { get; set; }
    
    [Column("IdDoctor")]
    public int IdDoctor { get; set; }
    
    [ForeignKey(nameof(IdDoctor))]
    public virtual Doctor Doctor { get; set; } = null!;
    
    [ForeignKey(nameof(IdPatient))]
    public virtual Patient Patient { get; set; } = null!;
    
    
}