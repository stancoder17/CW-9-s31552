namespace CW_9_s31552.Models.DTOs;

public class GetPrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public IEnumerable<GetMedicamentDto> Medicaments { get; set; } = null!;
    public GetDoctorDto Doctor { get; set; } = null!;
}