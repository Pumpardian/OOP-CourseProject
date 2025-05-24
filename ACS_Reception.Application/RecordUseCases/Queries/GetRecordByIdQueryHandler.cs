namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public class GetRecordByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecordByIdQuery, CardRecord>
    {
        public async Task<CardRecord> Handle(GetRecordByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.RecordRepository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
