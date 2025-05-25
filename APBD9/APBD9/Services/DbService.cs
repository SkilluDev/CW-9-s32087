using APBD9.Data;
using APBD9.DTOs;
using APBD9.Exceptions;
using APBD9.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD9.Services;

public interface IDbService
{
    Task<PrescriptionGetDto> CreatePrescriptionAsync(PrescriptionPostDto prescriptionData);
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
            Patient = new PatientGetDto()
            {
                FirstName = prescriptionData.Patient.FirstName,
                LastName = prescriptionData.Patient.LastName,
                Birthdate = prescriptionData.Patient.Birthdate,
                IdPatient = prescriptionData.Patient.IdPatient,
            },
            Doctor = new DoctorGetDto()
            {
                IdDoctor = prescriptionData.Doctor.IdDoctor,
                FirstName = prescriptionData.Doctor.FirstName,
                LastName = prescriptionData.Doctor.LastName,
                Email = prescriptionData.Doctor.Email,
            },
            Date = prescriptionData.Date,
            DueDate = prescriptionData.DueDate,
            Prescriptions = prescriptionData.Medicaments
        };
    }
}