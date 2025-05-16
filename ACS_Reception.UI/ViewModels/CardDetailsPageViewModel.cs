using ACS_Reception.Application.CardUseCases.Commands;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    [QueryProperty("Card", "Card")]
    public partial class CardDetailsPageViewModel(IMediator mediator) : ObservableObject
    {
        private readonly IMediator mediator = mediator;

        public ObservableCollection<CardRecord> Records { get; set; } = [];

        [ObservableProperty] Card card;
        [ObservableProperty] CardRecord? selectedRecord;

        [RelayCommand] async Task UpdateRecordList() => await GetRecords();
        [RelayCommand] async Task ShowRecordDetails(CardRecord record) => await GoToDetailsPage(record);
        [RelayCommand] async Task EditCard() => await GoToEditPage(new EditCardCommand() { Card = Card });
        [RelayCommand] async Task DeleteCard() => await RemoveCard(Card);

        [RelayCommand]
        async Task GetCardById()
        {
            Card = await mediator.Send(new GetCardByIdQuery(Card.Id));

            if (Card is null)
            {
                return;
            }
        }

        private async Task RemoveCard(Card card)
        {
            await mediator.Send(new DeleteCardCommand(card));
            await Shell.Current.GoToAsync("..");
        }

        private async Task GoToEditPage(IAddOrEditCardRequest request)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Request", request }
                };

            await Shell.Current.GoToAsync(nameof(AddOrEditCardPage), parameters);
        }

        private async Task GoToDetailsPage(CardRecord record)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "CardRecord", record }
                };

            await Shell.Current.GoToAsync(nameof(RecordDetailsPage), parameters);
        }

        public async Task GetRecords()
        {
            var records = await mediator.Send(new GetRecordsByCardIDQuery(Card.Id));

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Records.Clear();

                foreach (var record in records)
                {
                    Records.Add(record);
                }
            });
        }
    }
}
