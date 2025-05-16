namespace ACS_Reception.Application.CardUseCases.Queries
{
    public class GetCardByLastNameAndYearQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCardByLastNameAndYearQuery, Card>
    {
        public async Task<Card> Handle(GetCardByLastNameAndYearQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.CardRepository.FirstOrDefaultAsync(c => c.BirthDate.Year == request.Year && c.LastName == request.LastName, cancellationToken);
        }
    }
}
