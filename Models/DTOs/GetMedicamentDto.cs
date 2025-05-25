namespace CW_9_s31552.Models.DTOs;

public class GetMedicamentDto
{
    public int IdMedicament { get; set; }
    public string Name { get; set; } = null!;
    public int? Dose { get; set; }
    public string Description { get; set; } = null!;
}