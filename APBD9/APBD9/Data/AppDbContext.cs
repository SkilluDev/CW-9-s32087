using APBD9.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed Doctors
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { IdDoctor = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com" },
            new Doctor { IdDoctor = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com" }
        );

        // Seed Patients
        modelBuilder.Entity<Patient>().HasData(
            new Patient { IdPatient = 1, FirstName = "Piotr", LastName = "Zielinski", Birthdate = new DateTime(1990, 5, 15) },
            new Patient { IdPatient = 2, FirstName = "Maria", LastName = "Wojcik", Birthdate = new DateTime(1985, 10, 22) }
        );

        // Seed Medicaments
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament { IdMedicament = 1, Name = "Apap", Description = "Lek przeciwbólowy", Type = "Tabletki" },
            new Medicament { IdMedicament = 2, Name = "Ibuprom", Description = "Lek przeciwzapalny", Type = "Kapsułki" },
            new Medicament { IdMedicament = 3, Name = "Rutinoscorbin", Description = "Wspiera odporność", Type = "Tabletki" }
        );

        // Seed Prescriptions
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription
            {
                IdPrescription = 1,
                Date = new DateTime(2023, 1, 10),
                DueDate = new DateTime(2023, 2, 10),
                IdPatient = 1,
                IdDoctor = 1
            },
            new Prescription
            {
                IdPrescription = 2,
                Date = new DateTime(2023, 3, 5),
                DueDate = new DateTime(2023, 4, 5),
                IdPatient = 2,
                IdDoctor = 2
            },
            new Prescription
            {
                IdPrescription = 3,
                Date = new DateTime(2023, 1, 15),
                DueDate = new DateTime(2023, 3, 15),
                IdPatient = 1,
                IdDoctor = 1
            }
        );

        // Seed PrescriptionMedicaments (junction table)
        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 2, Details = "2 razy dziennie po posiłku" },
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 1, Details = "1 raz dziennie wieczorem" },
            new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 3, Dose = 3, Details = "3 razy dziennie" },
            new PrescriptionMedicament { IdPrescription = 3, IdMedicament = 1, Dose = 1, Details = "1 raz dziennie rano" }
        );

        // Configure many-to-many relationship for PrescriptionMedicament
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);
    }


}