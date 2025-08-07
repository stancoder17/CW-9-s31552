# Prescription API – .NET Core + EF Core (Code First)

This project was created as part of my studies in computer science at **Polish-Japanese Academy of Information Technology.**  
The goal was to practice the **Code First** approach using the Entity Framework Core library.

The application allows adding and retrieving medical prescriptions, with basic validation and business rules.  
It was built as a **backend-only** project — there is no frontend or user interface.

## Endpoints

### Adding a prescription

Adds a new **prescription** for a patient. The request body must include:
- patient data
- a list of prescribed medicaments (up to 10)
- prescriptions' due date

**Validation:**

- If the patient does not exist, a new patient is added
- If any medicament does not exist in the database, an error is returned
- A prescription cannot include more than 10 medicaments
- `DueDate` must not be in the past

### Getting patient details
Returns full information about a selected patient, including:
- personal data
- all related prescriptions
- all prescribed medicaments
- information about doctors
Prescriptions are sorted by 'DueDate'.

The code is separated into different **layers**:
1. Due to the small size of the project, **data access** and **business logic** are handled in the same layer.
2. **Controller** layer: handles HTTP requests and responses

# CW-9-s31552
