using CW_9_s31552.DAL;
using CW_9_s31552.Exceptions;
using CW_9_s31552.Models;
using CW_9_s31552.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s31552.Services;

public class DbService(NfzDbContext dbContext) : IDbService
{
    public async Task<GetPatientWithDetailsDto> GetPatientWithDetailsAsync(int idPatient, CancellationToken cancellationToken)
    {
        var result = await dbContext.Patients
            .Select(p => new GetPatientWithDetailsDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.Birthdate,
                Prescriptions = p.Prescriptions.Select(pr => new GetPrescriptionDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Medicaments = pr.PrescriptionMedicaments.Select(prm => new GetMedicamentDto
                        {
                            IdMedicament = prm.IdMedicament,
                            Name = prm.Medicament.Name,
                            Dose = prm.Dose,
                            Description = prm.Medicament.Description
                        }).ToList(),
                        Doctor = new GetDoctorDto
                        {
                            IdDoctor = pr.IdDoctor,
                            FirstName = pr.Doctor.FirstName
                        }
                    }).OrderBy(pr => pr.DueDate)
                    .ToList()
            }).FirstOrDefaultAsync(p => p.IdPatient == idPatient, cancellationToken);

        if (result == null)
            throw new NotFoundException($"Patient with id {idPatient} not found");
        
        return result;
    }

    public async Task<AddPrescriptionDto> AddPrescriptionAsync(AddPrescriptionDto prescriptionDto, CancellationToken cancellationToken)
    {
        // Medicaments list check
        if (!prescriptionDto.Medicaments.Any())
            throw new BadRequestException("Medicaments are required to add a prescription.");
        if (prescriptionDto.Medicaments.Count() > 10) 
            throw new BadRequestException("Maximum 10 medicaments can be added to the prescription.");

        // Medicaments existence check
        foreach (var medicamentDto in prescriptionDto.Medicaments)
        {
            var idMedicament = medicamentDto.IdMedicament;
            var medicament = dbContext.Medicaments.FirstOrDefaultAsync(m => m.IdMedicament == idMedicament, cancellationToken);
            if (medicament == null)
            {
                throw new BadRequestException($"A medicament with id {idMedicament} not found.");
            }
        }
        
        // Doctor existence check
        if (await dbContext.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == prescriptionDto.IdDoctor, cancellationToken) == null)
            throw new BadRequestException($"Doctor with id {prescriptionDto.IdDoctor} not found.");
        
        // Due date check
        if (prescriptionDto.DueDate < prescriptionDto.Date)
            throw new BadRequestException("Incorrect due date.");

        var prescription = new Prescription
        {
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            IdPatient = prescriptionDto.Patient.IdPatient,
            IdDoctor = prescriptionDto.IdDoctor
        };
        await dbContext.Prescriptions.AddAsync(prescription, cancellationToken);

        foreach (var medicamentDto in prescriptionDto.Medicaments)
        {
            var prescriptionMedicament = new PrescriptionMedicament
            {
                IdMedicament = medicamentDto.IdMedicament,
                IdPrescription = prescription.IdPrescription,
                Dose = medicamentDto.Dose,
                Details = medicamentDto.Description
            };
            await dbContext.PrescriptionMedicaments.AddAsync(prescriptionMedicament, cancellationToken);
        }

        var patient = await dbContext.Patients.Include(patient => patient.Prescriptions).FirstOrDefaultAsync(p => p.IdPatient == prescriptionDto.Patient.IdPatient, cancellationToken)
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = prescriptionDto.Patient.IdPatient,
                FirstName = prescriptionDto.Patient.FirstName,
                LastName = prescriptionDto.Patient.LastName,
                Birthdate = prescriptionDto.Patient.Birthdate,
                Prescriptions = []
            };
            patient.Prescriptions.Add(prescription);
            await dbContext.Patients.AddAsync(patient, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new GetPatientWithDetailsDto
        {
            IdPatient = prescriptionDto.Patient.IdPatient,
            FirstName = prescriptionDto.Patient.FirstName,
            LastName = prescriptionDto.Patient.LastName,
            Birthdate = prescriptionDto.Patient.Birthdate,
            Prescriptions = patient.Prescriptions
        }
    }
}