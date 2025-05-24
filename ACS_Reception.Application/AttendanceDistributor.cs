using ACS_Reception.Application.DoctorUseCases.Strategies;
using ACS_Reception.Domain.DTOs;

namespace ACS_Reception.Application
{
    public class AttendanceDistributor(IMediator mediator)
    {
        private readonly Dictionary<AttendanceReason, DoctorType> doctorForwarding = new()
        {
            { AttendanceReason.TherapistVisit, DoctorType.Therapist },
            { AttendanceReason.Analyzis, DoctorType.LabDiagnostic },
            { AttendanceReason.MentalProblem, DoctorType.Psychologist },
        };

        private readonly Dictionary<AttendanceReason, IDoctorStrategy> strategyForwarding = new()
        {
            { AttendanceReason.TherapistVisit, new TherapistStrategy(mediator) },
            { AttendanceReason.Analyzis, new AnalyzisStrategy(mediator) },
            { AttendanceReason.MentalProblem, new PsychologistStrategy(mediator) },
        };

        public DoctorType ForwardToDoctor(AttendanceReason attendanceReason)
        {
            return doctorForwarding[attendanceReason];
        }

        public async Task<string> ForwardToStrategy(AttendanceInfo attendanceInfo)
        {
            return await strategyForwarding[attendanceInfo.AttendanceReason!.Value].Attendance(attendanceInfo);
        }
    }
}
