namespace CW_9_s31552.Models.DTOs;

public class GetPatientWithDetailsDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public IEnumerable<GetPrescriptionDto> Prescriptions { get; set; } = null!;
}