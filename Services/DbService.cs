using CW_9_s31552.DAL;
using CW_9_s31552.Exceptions;
using CW_9_s31552.Models;
using CW_9_s31552.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s31552.Services;

public class DbService(NfzDbContext dbContext) : IDbService
{
    public async Task<GetPrescriptionDto> AddPrescriptionAsync(AddPrescriptionDto prescriptionDto, CancellationToken cancellationToken)
    {
        // Medicaments list check
        ValidateMedicaments(prescriptionDto.Medicaments);

        // Medicaments existence check
        {
            var ids = prescriptionDto.Medicaments.Select(m => m.IdMedicament).ToList();
            var existingIds = await dbContext.Medicaments
                .Where(m => ids.Contains(m.IdMedicament))
                .Select(m => m.IdMedicament)
                .ToListAsync(cancellationToken);
            var missing = ids.Except(existingIds).ToList();
            if (missing.Count != 0)
                throw new NotFoundException($"Medicaments with IDs [{string.Join(", ", missing)}] not found.");
        }

        // Doctor existence check
        if (!await dbContext.Doctors.AnyAsync(d => d.IdDoctor == prescriptionDto.IdDoctor, cancellationToken))
            throw new NotFoundException($"Doctor with id {prescriptionDto.IdDoctor} not found.");
        
        // Due date check
        ValidateDates(prescriptionDto.DueDate, prescriptionDto.Date);

        
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken); // Many requests at different times - transaction needed
        try
        {
            // Database: add Patient if not exists
            var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.IdPatient == prescriptionDto.Patient.IdPatient, cancellationToken);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = prescriptionDto.Patient.FirstName,
                    LastName = prescriptionDto.Patient.LastName,
                    Birthdate = prescriptionDto.Patient.Birthdate,
                };
                await dbContext.Patients.AddAsync(patient, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken); // Save changes here because IdPatient (which is auto-generated) is available only after saving changes.
            }

            // Database: add Prescription
            var prescriptionToAdd = new Prescription
            {
                Date = prescriptionDto.Date,
                DueDate = prescriptionDto.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = prescriptionDto.IdDoctor,
            };
            await dbContext.Prescriptions.AddAsync(prescriptionToAdd, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken); // Same reason as above, but for IdPrescription.

            // Database: add PrescriptionMedicaments
            List<GetMedicamentDto> medicamentDtosForReturn = [];
            foreach (var medicamentDto in prescriptionDto.Medicaments)
            {
                var prescriptionMedicamentToAdd = new PrescriptionMedicament
                {
                    IdMedicament = medicamentDto.IdMedicament,
                    IdPrescription = prescriptionToAdd.IdPrescription,
                    Dose = medicamentDto.Dose,
                    Details = medicamentDto.Description
                };
                medicamentDtosForReturn.Add(new GetMedicamentDto // prepare GetMedicamentDto for return
                {
                    IdMedicament = medicamentDto.IdMedicament,
                    Name = dbContext.Medicaments.FirstOrDefault(m => m.IdMedicament == medicamentDto.IdMedicament)!.Name,
                    Dose = medicamentDto.Dose,
                    Description = medicamentDto.Description,
                });
                await dbContext.PrescriptionMedicaments.AddAsync(prescriptionMedicamentToAdd, cancellationToken);
            }
            await dbContext.SaveChangesAsync(cancellationToken); 
            
            // Commit
            await transaction.CommitAsync(cancellationToken);
            
            // Return GetPrescriptionDto
            return new GetPrescriptionDto
            {
                IdPrescription = prescriptionToAdd.IdPrescription,
                Date = prescriptionToAdd.Date,
                DueDate = prescriptionToAdd.DueDate,
                Medicaments = medicamentDtosForReturn,
                Doctor = new GetDoctorDto
                {
                    IdDoctor = prescriptionDto.IdDoctor,
                    FirstName = dbContext.Doctors.FirstOrDefault(d => d.IdDoctor == prescriptionDto.IdDoctor)!.FirstName,
                }
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

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

    private static void ValidateMedicaments(IEnumerable<AddMedicamentDto> medicaments)
    {
        medicaments = medicaments.ToList();
        if (!medicaments.Any())
            throw new BadRequestException("Medicaments are required to add a prescription.");
        if (medicaments.Count() > 10)
            throw new BadRequestException("Maximum 10 medicaments can be added to the prescription.");
    }

    private static void ValidateDates(DateTime dueDate, DateTime date)
    {
        if (dueDate < date)
            throw new BadRequestException("Date cannot be bigger than due date.");
    }
}