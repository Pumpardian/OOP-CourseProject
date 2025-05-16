namespace ACS_Reception.Domain.Entities.CardRecords
{
    public class DoctorAttendance(DateTime date, string note, int cardId, string doctor) : CardRecord(date, note, cardId)
    {
        public string Doctor { get; } = doctor;

        public override string GetRecordName
        {
            get => "Doctor Attendance";
        }
    }
}