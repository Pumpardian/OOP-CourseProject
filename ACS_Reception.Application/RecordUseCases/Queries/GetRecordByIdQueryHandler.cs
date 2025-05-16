namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public class GetRecordByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecordByIDQuery, CardRecord>
    {
        public async Task<CardRecord> Handle(GetRecordByIDQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.RecordRepository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
