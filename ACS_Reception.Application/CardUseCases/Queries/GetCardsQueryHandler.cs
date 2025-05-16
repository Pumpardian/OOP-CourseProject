namespace ACS_Reception.Application.CardUseCases.Queries
{
    public class GetCardsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCardsQuery, IEnumerable<Card>>
    {
        public async Task<IEnumerable<Card>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.CardRepository.ListAllAsync(cancellationToken);
        }
    }
}
