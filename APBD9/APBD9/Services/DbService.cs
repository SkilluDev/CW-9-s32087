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
            Doctor = prescriptionData.Doctor,
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
            Prescriptions = data.Prescriptions.Join(data.Doctors,p=>p.IdDoctor,d=>d.IdDoctor, (p,d)=>new {p,d}).
                Where(o => o.p.IdPatient == id).Select(o=>new PrescriptionGetDto()
                {
                IdPrescription = o.p.IdPrescription,
                Date = o.p.Date,
                DueDate = o.p.Date,
                Doctor = new DoctorGetDto()
                {
                    IdDoctor = o.d.IdDoctor,
                    FirstName = o.d.FirstName,
                    LastName = o.d.LastName,
                    Email = o.d.Email,
                },
                IdPatient = o.p.IdPatient,
                Medicaments = o.p.PrescriptionMedicaments.Join(data.Medicaments,p=>p.IdMedicament,m=>m.IdMedicament,(p,m)=>new {p,m}).Select(o2 => new PrescriptionMedicamentGetDto()
                {
                    IdMedicament = o2.p.IdMedicament,
                    Dose = o2.p.Dose,
                    Description = o2.p.Details,
                    
                    Medicament = new MedicamentGetDto(){
                        IdMedicament = o2.m.IdMedicament,
                        Description = o2.m.Description,
                        Name = o2.m.Name,
                        Type = o2.m.Type,
                    }
                    
                }).ToList(),
                
            }).OrderBy(p=>p.DueDate).ToList()
        };

    }
}