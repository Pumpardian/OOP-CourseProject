using ACS_Reception.Domain.Entities;

namespace ACS_Reception.Domain.DTOs
{
    public enum AttendanceReason
    {
        TherapistVisit,
        Analyzis,
        MentalProblem
    }

    public class AttendanceInfo()
    {
        public AttendanceReason? AttendanceReason { get; set; }
        public Doctor? Doctor { get; set; }
        public Card? Card { get; set; }
    }
}
