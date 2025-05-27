using CW_9_s31552.Models.DTOs;

namespace CW_9_s31552.Services;

public interface IDbService
{
    public Task<GetPrescriptionDto> AddPrescriptionAsync(AddPrescriptionDto prescriptionDto, CancellationToken cancellationToken);
    public Task<GetPatientWithDetailsDto> GetPatientWithDetailsAsync(int idPatient, CancellationToken cancellationToken);
}