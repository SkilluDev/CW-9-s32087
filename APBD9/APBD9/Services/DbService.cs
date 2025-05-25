using APBD9.Data;
using APBD9.DTOs;
using APBD9.Exceptions;
using APBD9.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Services;

public interface IDbService
{
    Task<PrescriptionGetDto> CreatePrescriptionAsync(PrescriptionPostDto prescriptionData);
    Task<PatientGetDto?> GetPatientAsync(int id);
}

public class DbService(AppDbContext data) : IDbService
{
    public async Task<PrescriptionGetDto> CreatePrescriptionAsync(PrescriptionPostDto prescriptionData)
    {
        var patient = await data.Patients.FirstOrDefaultAsync(g => g.IdPatient == prescriptionData.Patient.IdPatient);
        if (patient is null)
        {
            data.Patients.Add(new Patient
            {
                FirstName = prescriptionData.Patient.FirstName,
                LastName = prescriptionData.Patient.LastName,
                Birthdate = prescriptionData.Patient.Birthdate,
            });
        }
        
        var doctor = await data.Doctors.FirstOrDefaultAsync(g => g.IdDoctor == prescriptionData.Doctor.IdDoctor);
        if (doctor is null)
        {
            data.Doctors.Add(new Doctor()
            {
                FirstName = prescriptionData.Doctor.FirstName,
                LastName = prescriptionData.Doctor.LastName,
                Email = prescriptionData.Doctor.Email,
            });
        }

        if (prescriptionData.DueDate < prescriptionData.Date)
        {
            throw new ArgumentException("Due date cannot be greater than prescription date");
        }

        var prescription = new Prescription
        {
            Date = prescriptionData.Date,
            DueDate = prescriptionData.DueDate,
            IdPatient = prescriptionData.Patient.IdPatient,
            IdDoctor = prescriptionData.Doctor.IdDoctor,
            PrescriptionMedicaments = prescriptionData.Medicaments.Select(medicament => new PrescriptionMedicament()
                {
                    IdMedicament = medicament.IdMedicament,
                    Dose = medicament.Dose,
                    Details = medicament.Description
                }
            ).ToList()
        };

        // Add new student to the db context, and save all changes.
        await data.Prescriptions.AddAsync(prescription);
        await data.SaveChangesAsync();


        // Return created records to the controller.
        return new PrescriptionGetDto
        {
            IdPrescription = prescription.IdPrescription,
            IdPatient = prescriptionData.Patient.IdPatient,
            IdDoctor = prescriptionData.Doctor.IdDoctor,
            Date = prescriptionData.Date,
            DueDate = prescriptionData.DueDate,
            Medicaments = prescriptionData.Medicaments
        };
    }

    public async Task<PatientGetDto?> GetPatientAsync(int id)
    {
        var patient = await data.Patients.FirstOrDefaultAsync(g => g.IdPatient == id);
        if (patient is null)
        {
            throw new NotFoundException($"Patient with id '{ id }' not found");
        }

        return new PatientGetDto()
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = data.Prescriptions.
                Where(p => p.IdPatient == id).Select(p=>new PrescriptionGetDto()
                {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.Date,
                IdDoctor = p.IdDoctor,
                IdPatient = p.IdPatient,
                Medicaments = p.PrescriptionMedicaments.Select(medicament => new PrescriptionMedicamentGetDto()
                {
                    IdMedicament = medicament.IdMedicament,
                    Dose = medicament.Dose,
                    Description = medicament.Details
                    
                }).ToList(),
                
            }).OrderBy(p=>p.DueDate).ToList()
        };

    }
}