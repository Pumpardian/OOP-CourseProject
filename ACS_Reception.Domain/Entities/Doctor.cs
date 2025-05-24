using MongoDB.EntityFrameworkCore;

namespace ACS_Reception.Domain.Entities
{
    public enum DoctorType
    {
        LabDiagnostic,
        Psychologist,
        Therapist
    }

    [Collection("Doctors")]
    public class Doctor : Entity
    {
        public string FirstName { get; set; } = "First Name";
        public string LastName { get; set; } = "Last Name";
        public int CabinetNumber { get; set; } = 0;
        public DoctorType DoctorType { get; set; } = default;

        public Doctor() { }

        public Doctor(string firstName, string lastName, int cabinetNumber, DoctorType doctorType)
        {
            FirstName = firstName;
            LastName = lastName;
            CabinetNumber = cabinetNumber;
            DoctorType = doctorType;
        }

        public override string ToString()
        {
            return $"{LastName} {FirstName}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Doctor)
            {
                return false;
            }

            if (Id != (obj as Doctor)!.Id ||
                FirstName != (obj as Doctor)!.FirstName ||
                LastName != (obj as Doctor)!.LastName ||
                CabinetNumber != (obj as Doctor)!.CabinetNumber ||
                DoctorType != (obj as Doctor)!.DoctorType)
            {
                return false;
            }

            return true;
        }
    }
}
