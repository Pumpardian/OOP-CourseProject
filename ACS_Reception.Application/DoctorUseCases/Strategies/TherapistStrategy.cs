using ACS_Reception.Application.RecordUseCases.Commands;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.Domain.DTOs;
using ACS_Reception.Domain.Entities.CardRecords;

namespace ACS_Reception.Application.DoctorUseCases.Strategies
{
    public class TherapistStrategy(IMediator mediator) : IDoctorStrategy
    {
        private readonly IMediator mediator = mediator;



        public async Task<string> Attendance(AttendanceInfo attendanceInfo)
        {
            GetRecordsByCardIdQuery query = new(attendanceInfo.Card!.Id);
            List<CardRecord> records = [.. await mediator.Send(query)];

            Analyzis? lastAnalyzis;
            SickList? sickList;
            try
            {
                lastAnalyzis = records.OfType<Analyzis>().Where(a => DateTime.Now.Subtract(a.Date) < TimeSpan.FromHours(1))
                    .Aggregate((curMax, x) => (curMax == null || (x.Date) < curMax.Date ? x : curMax));

                sickList = records.OfType<SickList>()
                    .Aggregate((curMax, x) => (curMax == null || (x.Date) < curMax.Date ? x : curMax));
            }
            catch (Exception)
            {
                sickList = null;
                lastAnalyzis = null;
            }

            string response = String.Empty;

            if (lastAnalyzis != null && 
                !records.OfType<Check>()
                .Where(c => c.DoctorType == DoctorType.Therapist && c.Date > lastAnalyzis.Date).Any())
            {
                response += $"{lastAnalyzis.GetInfo} \n";

                Prescription prescription;
                switch (lastAnalyzis.AnalizysResults)
                {
                    case AnalizysResults.Satisfactory:
                        response += $"Go get some drops or srays. If you'll feel worse ask for some light medicine in pharmacy. Keep your routine calm and slow-paced.\n";
                        break;
                    case AnalizysResults.Norm:
                        response += $"Results are excellent. If your throat or nose are sick, then go get some drops or srays for that matter.\n";
                        break;
                    case AnalizysResults.Acceptable:
                        response += $"You should'nt overload your routine. Its better to stay at home. I'll prescribe some medicines.\n";

                        prescription = new(attendanceInfo.Card.Id, DateTime.Now,
                            $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}",
                            "AnHeadachein", Random.Shared.NextDouble());
                        await mediator.Send(new AddRecordCommand() { CardRecord = prescription });

                        prescription = new(attendanceInfo.Card.Id, DateTime.Now,
                            $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}",
                            "BodyAnPressurein", Random.Shared.NextDouble());
                        await mediator.Send(new AddRecordCommand() { CardRecord = prescription });

                        break;
                    case AnalizysResults.Unsatisfactory:
                        response += $"You're in a very bad state. Minimize your workload and routine. I'll prescribe the medicines.\n";

                        prescription = new(attendanceInfo.Card.Id, DateTime.Now,
                            $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}",
                            "AnHeadachein", Random.Shared.NextDouble());
                        await mediator.Send(new AddRecordCommand() { CardRecord = prescription });

                        prescription = new(attendanceInfo.Card.Id, DateTime.Now,
                            $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}",
                            "BodyAnPressurein", Random.Shared.NextDouble());
                        await mediator.Send(new AddRecordCommand() { CardRecord = prescription });

                        prescription = new(attendanceInfo.Card.Id, DateTime.Now,
                            $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}",
                            "TempDownzill", Random.Shared.NextDouble());
                        await mediator.Send(new AddRecordCommand() { CardRecord = prescription });

                        break;
                    default:
                        break;
                }
            }
            else if (lastAnalyzis != null && sickList != null)
            {
                response += "You treatment has ended.";

                sickList.EndDate = DateTime.Now;

                await mediator.Send(new EditRecordCommand() { CardRecord = sickList });
            }
            else if (sickList != null)
            {
                response += "You treatment has ended. We'll write you a new sick list, as we lost previous one.";

                sickList = new(attendanceInfo.Card.Id, DateTime.Now,
                    $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}")
                {
                    EndDate = DateTime.Now
                };

                await mediator.Send(new AddRecordCommand() { CardRecord = sickList });
            }
            else
            {
                response += "You should pass analyzis to determine your treatment.";

                sickList = new(attendanceInfo.Card.Id, DateTime.Now,
                    $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}");

                await mediator.Send(new AddRecordCommand() { CardRecord = sickList });
            }

            Check check = new(attendanceInfo.Card.Id, DateTime.Now,
                $"{attendanceInfo.Doctor!.LastName} {attendanceInfo.Doctor!.FirstName}", DoctorType.Therapist, response);

            await mediator.Send(new AddRecordCommand() { CardRecord = check });
            return response;
        }
    }
}
