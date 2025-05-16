using ACS_Reception.Application.DoctorUseCases.Queries;

namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public class GetRecordsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecordsQuery, IEnumerable<CardRecord>>
    {
        public async Task<IEnumerable<CardRecord>> Handle(GetRecordsQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.RecordRepository.ListAllAsync(cancellationToken);
        }
    }
}
