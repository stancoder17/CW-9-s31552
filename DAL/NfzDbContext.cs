using CW_9_s31552.Models;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s31552.DAL;

public class NfzDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; } // klasa zarejestrowana w DbContext jako model
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }

    protected NfzDbContext()
    {
    }

    public NfzDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { IdDoctor = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new Doctor { IdDoctor = 2, FirstName = "Emily", LastName = "Clark", Email = "emily.clark@example.com" },
            new Doctor { IdDoctor = 3, FirstName = "Robert", LastName = "Jones", Email = "robert.jones@example.com" }
        );

        modelBuilder.Entity<Patient>().HasData(
            new Patient { IdPatient = 1, FirstName = "Alice", LastName = "Smith", Birthdate = new DateTime(1990, 1, 1) },
            new Patient { IdPatient = 2, FirstName = "Bob", LastName = "Johnson", Birthdate = new DateTime(1985, 3, 15) },
            new Patient { IdPatient = 3, FirstName = "Carol", LastName = "White", Birthdate = new DateTime(2000, 7, 20) },
            new Patient { IdPatient = 4, FirstName = "David", LastName = "Brown", Birthdate = new DateTime(1975, 12, 5) },
            new Patient { IdPatient = 5, FirstName = "Eve", LastName = "Taylor", Birthdate = new DateTime(1995, 9, 30) }
        );

        modelBuilder.Entity<Prescription>().HasData(
            new Prescription { IdPrescription = 1, Date = new DateTime(2024, 5, 1), DueDate = new DateTime(2024, 5, 15), IdDoctor = 1, IdPatient = 1 },
            new Prescription { IdPrescription = 2, Date = new DateTime(2024, 5, 5), DueDate = new DateTime(2024, 5, 20), IdDoctor = 2, IdPatient = 2 },
            new Prescription { IdPrescription = 3, Date = new DateTime(2024, 5, 10), DueDate = new DateTime(2024, 5, 25), IdDoctor = 3, IdPatient = 3 },
            new Prescription { IdPrescription = 4, Date = new DateTime(2024, 5, 12), DueDate = new DateTime(2024, 5, 26), IdDoctor = 1, IdPatient = 4 },
            new Prescription { IdPrescription = 5, Date = new DateTime(2024, 5, 15), DueDate = new DateTime(2024, 5, 30), IdDoctor = 2, IdPatient = 5 },
            new Prescription { IdPrescription = 6, Date = new DateTime(2024, 5, 17), DueDate = new DateTime(2024, 6, 1), IdDoctor = 3, IdPatient = 1 },
            new Prescription { IdPrescription = 7, Date = new DateTime(2024, 5, 18), DueDate = new DateTime(2024, 6, 2), IdDoctor = 1, IdPatient = 2 },
            new Prescription { IdPrescription = 8, Date = new DateTime(2024, 5, 20), DueDate = new DateTime(2024, 6, 5), IdDoctor = 3, IdPatient = 4 }
        );
        
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Pain reliever", Type = "Tablet" },
            new Medicament { IdMedicament = 2, Name = "Amoxicillin", Description = "Antibiotic", Type = "Capsule" },
            new Medicament { IdMedicament = 3, Name = "Ibuprofen", Description = "Anti-inflammatory", Type = "Tablet" },
            new Medicament { IdMedicament = 4, Name = "Cetirizine", Description = "Antihistamine", Type = "Tablet" },
            new Medicament { IdMedicament = 5, Name = "Omeprazole", Description = "Stomach acid reducer", Type = "Capsule" }
        );

        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 500, Details = "Take twice a day" },
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 250, Details = "After meals" },
            new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 2, Dose = 500, Details = "3 times daily" },
            new PrescriptionMedicament { IdPrescription = 3, IdMedicament = 3, Dose = 400, Details = "Before sleep" },
            new PrescriptionMedicament { IdPrescription = 4, IdMedicament = 1, Dose = 500, Details = "Twice daily" },
            new PrescriptionMedicament { IdPrescription = 5, IdMedicament = 4, Dose = 10, Details = "Once a day" },
            new PrescriptionMedicament { IdPrescription = 6, IdMedicament = 5, Dose = 20, Details = "Morning only" },
            new PrescriptionMedicament { IdPrescription = 7, IdMedicament = 3, Dose = 200, Details = "Every 8 hours" },
            new PrescriptionMedicament { IdPrescription = 8, IdMedicament = 4, Dose = 10, Details = "As needed" }
        );

    }
}