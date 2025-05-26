using System.ComponentModel.DataAnnotations;

namespace CW_9_s31552.Models.DTOs;

public class AddPrescriptionDto
{
    [Required]
    public AddPatientDto Patient { get; set; } = null!;

    [Required]
    public IEnumerable<AddMedicamentDto> Medicaments { get; set; } = null!;
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    [Required]
    public int IdDoctor { get; set; }
}