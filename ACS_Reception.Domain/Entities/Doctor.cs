using ACS_Reception.Domain.Abstractions;

namespace ACS_Reception.Domain.Entities
{
    public enum DoctorType
    {
        GeneralPractitioner,
        Ophthalmologist,
        LabDiagnostic
    }

    public class Doctor(string firstName, string lastName, int cabinetNumber, DoctorType doctorType) : Entity
    {
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public int CabinetNumber { get; set; } = cabinetNumber;
        public DoctorType DoctorType { get; set; } = doctorType;
    }
}
