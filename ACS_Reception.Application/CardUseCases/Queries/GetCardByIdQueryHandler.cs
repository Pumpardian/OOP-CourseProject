namespace ACS_Reception.Application.CardUseCases.Queries
{
    public class GetCardByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCardByIdQuery, Card>
    {
        public async Task<Card> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.CardRepository.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        }
    }
}
