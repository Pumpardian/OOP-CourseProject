using ACS_Reception.Domain.DTOs;

namespace ACS_Reception.Domain.Abstractions
{
    public interface IDoctorStrategy
    {
        Task<string> Attendance(AttendanceInfo attendanceInfo);
    }
}
