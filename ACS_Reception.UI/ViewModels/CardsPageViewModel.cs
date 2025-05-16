using ACS_Reception.Application.CardUseCases.Commands;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    public partial class CardsPageViewModel(IMediator mediator) : ObservableObject
    {
        private readonly IMediator mediator = mediator;

        public ObservableCollection<Card> Cards { get; set; } = [];

        [ObservableProperty] Card? selectedCard;

        [RelayCommand] async Task UpdateCardList() => await GetCards();
        [RelayCommand] async Task ShowCardDetails(Card card) => await GoToDetailsPage(card);
        [RelayCommand] async Task AddCard() =>
            await GoToAddPage(new AddCardCommand() { Card = new Card("first name", "last name", DateOnly.MinValue) });

        private async Task GoToDetailsPage(Card card)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Card", card }
                };

            await Shell.Current.GoToAsync(nameof(CardDetailsPage), parameters);
        }

        private async Task GoToAddPage(IRequest request)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Request", request }
                };

            await Shell.Current.GoToAsync(nameof(AddOrEditCardPage), parameters);
        }

        public async Task GetCards()
        {
            var cards = await mediator.Send(new GetCardsQuery());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Cards.Clear();

                foreach (var card in cards)
                {
                    Cards.Add(card);
                }
            });
        }
    }
}
