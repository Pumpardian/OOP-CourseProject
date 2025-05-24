namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public class GetRecordsByCardIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRecordsByCardIdQuery, IEnumerable<CardRecord>>
    {
        public async Task<IEnumerable<CardRecord>> Handle(GetRecordsByCardIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.RecordRepository.ListAsync((record) => record.CardId == request.CardId, cancellationToken);
        }
    }
}
