using ACS_Reception.Domain.Entities;

namespace ACS_Reception.Tests.DomainTests
{
    public class DoctorTests
    {
        [Fact]
        public void Constructor_Doctor_ValidData()
        {
            string firstName = "Name";
            string lastName = "Surname";
            int cabinetNumber = 102;
            DoctorType doctorType = DoctorType.Psychologist;
            var doctor = new Doctor(firstName, lastName, cabinetNumber, doctorType);
            Assert.Equal(firstName, doctor.FirstName);
            Assert.Equal(lastName, doctor.LastName);
            Assert.Equal(cabinetNumber, doctor.CabinetNumber);
            Assert.Equal(doctorType, doctor.DoctorType);
        }

        [Fact]
        public void Doctor_SettersAvailable()
        {
            string firstName = "Name";
            string lastName = "Surname";
            int cabinetNumber = 102;
            DoctorType doctorType = DoctorType.Psychologist;
            var doctor = new Doctor(firstName, lastName, cabinetNumber, doctorType);

            string firstName2 = "Name2";
            string lastName2 = "Surname2";
            int cabinetNumber2 = 103;
            DoctorType doctorType2 = DoctorType.LabDiagnostic;

            doctor.FirstName = firstName2;
            doctor.LastName = lastName2;
            doctor.CabinetNumber = cabinetNumber2;
            doctor.DoctorType = doctorType2;

            Assert.Equal(firstName2, doctor.FirstName);
            Assert.Equal(lastName2, doctor.LastName);
            Assert.Equal(cabinetNumber2, doctor.CabinetNumber);
            Assert.Equal(doctorType2, doctor.DoctorType);
        }
    }
}
