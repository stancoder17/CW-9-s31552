using CW_9_s31552.DAL;
using CW_9_s31552.Exceptions;
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
}