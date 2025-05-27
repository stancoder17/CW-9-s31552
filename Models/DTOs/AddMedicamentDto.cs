namespace CW_9_s31552.Models.DTOs;

public class AddMedicamentDto
{
    public int IdMedicament { get; set; }
    public int? Dose { get; set; }
    public string Description { get; set; } = null!;
}