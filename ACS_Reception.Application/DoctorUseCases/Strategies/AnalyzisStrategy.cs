using ACS_Reception.Application.RecordUseCases.Commands;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.Domain.DTOs;
using ACS_Reception.Domain.Entities.CardRecords;

namespace ACS_Reception.Application.DoctorUseCases.Strategies
{
    public class AnalyzisStrategy(IMediator mediator) : IDoctorStrategy
    {
        private readonly IMediator mediator = mediator;

        public async Task<string> Attendance(AttendanceInfo attendanceInfo)
        {
            GetRecordsByCardIdQuery query = new(attendanceInfo.Card!.Id);
            List<CardRecord> records = [.. await mediator.Send(query)];

            if (records.OfType<Analyzis>().Where(a => DateTime.Now.Subtract(a.Date) < TimeSpan.FromHours(1)).Any())
            {
                return "There is no need to pass analizies too often, previous results are still valid.";
            }

            AnalizysResults analizysResults = (AnalizysResults) Random.Shared.Next(Enum.GetValues<AnalizysResults>().Length);

            Analyzis analyzis = new(attendanceInfo.Card.Id, DateTime.Now,
                $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}", analizysResults);

            await mediator.Send(new AddRecordCommand() { CardRecord = analyzis });
            return "You've passed your analyzis.";
        }
    }
}
