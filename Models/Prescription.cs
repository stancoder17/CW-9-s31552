using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CW_9_s31552.Models;

public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public DateTime DueDate { get; set; }
    
    public int IdPatient { get; set; }

    [ForeignKey(nameof(IdPatient))] // "gdy bedziemy pobierali to pole, zrob JOIN'a za pomoca pola w nameof()"
    public Patient Patient { get; set; } = null!; 

    public int IdDoctor { get; set; }

    [ForeignKey(nameof(IdDoctor))]
    public Doctor Doctor { get; set; } = null!;
}