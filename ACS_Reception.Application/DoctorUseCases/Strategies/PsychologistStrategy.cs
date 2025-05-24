using ACS_Reception.Application.RecordUseCases.Commands;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.Domain.DTOs;
using ACS_Reception.Domain.Entities.CardRecords;

namespace ACS_Reception.Application.DoctorUseCases.Strategies
{
    public class PsychologistStrategy(IMediator mediator) : IDoctorStrategy
    {
        private readonly IMediator mediator = mediator;

        private readonly List<string> noRecentChecksResponses =
            [
                "You should take things more easily, especially minor ones.",
                "Calm down, think, do - thats should always be in your mind."
            ];
        private readonly List<string> recentCheckResponses =
            [
                "You should take in consideration how the other person feels about your actions.",
                "Think of your surroundings, you should think how your actions will reflect on it."
            ];
        private readonly List<string> recentChecksResponses =
            [
                "Consider doing less stressful lifestyle. Don't overload yourself with multitasking.",
                "Think about getting job with calm flow. Don't take anything personally. Be more relaxed."
            ];

        public async Task<string> Attendance(AttendanceInfo attendanceInfo)
        {
            GetRecordsByCardIdQuery query = new(attendanceInfo.Card!.Id);
            List<CardRecord> records = [.. await mediator.Send(query)];

            int recentChecks = records.OfType<Check>().Where(a => DateTime.Now.Subtract(a.Date) < TimeSpan.FromDays(1) && a.DoctorType == DoctorType.Psychologist).Count();
            string response = recentChecks switch
            {
                0 => noRecentChecksResponses[Random.Shared.Next(noRecentChecksResponses.Count)],
                1 => recentCheckResponses[Random.Shared.Next(recentCheckResponses.Count)],
                _ => recentChecksResponses[Random.Shared.Next(recentChecksResponses.Count)],
            };

            Check check = new(attendanceInfo.Card.Id, DateTime.Now, $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}", DoctorType.Psychologist, response);
            await mediator.Send(new AddRecordCommand() { CardRecord = check });

            return response;
        }
    }
}
