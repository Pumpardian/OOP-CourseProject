using ACS_Reception.Application.CardUseCases.Commands;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Domain.Abstractions;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    public partial class CardsPageViewModel(IMediator mediator, ISerializer? serializer = null) : ObservableObject
    {
        private readonly IMediator mediator = mediator;
        private readonly ISerializer? serializer = serializer;
        private static readonly string[][] serializationFormats = [[".json"], [".xml"]];

        public ObservableCollection<Card> Cards { get; set; } = [];

        [ObservableProperty] bool isSerializationPluginConnected = false;
        [ObservableProperty] List<string> sortBy = [ "Default", "Last Name", "First Name", "Date"];
        [ObservableProperty] bool reverseSort = false;

        [RelayCommand] async Task UpdateSerializationStatus() => await Task.Run(() => IsSerializationPluginConnected = serializer is not null);
        [RelayCommand] async Task SerializeJson() => await SerializeCards(false);
        [RelayCommand] async Task DeserializeJson() => await DeserializeCards(false);
        [RelayCommand] async Task SerializeXml() => await SerializeCards(true);
        [RelayCommand] async Task DeserializeXml() => await DeserializeCards(true);

        [RelayCommand] async Task UpdateCardList() => await GetCards();
        [RelayCommand] async Task ShowCardDetails(Card card) => await GoToDetailsPage(card);
        [RelayCommand] async Task AddCard() =>
            await GoToAddPage(new AddCardCommand() { Card = new Card() });

        [RelayCommand] async Task Sort(int index) => await SortCollection(index);

        private async Task SortCollection(int index)
        {
            var sorted = index switch
            {
                0 => [.. ReverseSort ? Cards.OrderByDescending(c => c.Id) : Cards.OrderBy(c => c.Id)],
                1 => [.. ReverseSort ? Cards.OrderByDescending(c => c.LastName) : Cards.OrderBy(c => c.LastName)],
                2 => [.. ReverseSort ? Cards.OrderByDescending(c => c.FirstName) :Cards.OrderBy(c => c.FirstName)],
                3 => [.. ReverseSort ? Cards.OrderByDescending(c => c.BirthDate) : Cards.OrderBy(c => c.BirthDate)],
                _ => new List<Card>(),
            };


            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Cards.Clear();

                foreach (var card in sorted)
                {
                    Cards.Add(card);
                }
            });
        }

        private async Task SerializeCards(bool xml)
        {
            var result = await FolderPicker.PickAsync("");
            if (!result.IsSuccessful)
            {
                return;
            }

            var name = await Shell.Current.DisplayPromptAsync("Saving", "Enter Filename:", "OK", "Cancel", initialValue: "Cards");
            name += xml ? ".xml" : ".json";

            name = Path.Combine(result.Folder.Path, name);
            if (xml)
            {
                await serializer!.SerializeXml(Cards.ToList(), name);
            }
            else
            {
                await serializer!.SerializeJson(Cards.ToList(), name);
            }
        }

        private async Task DeserializeCards(bool xml)
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, serializationFormats[xml ? 1 : 0] }
                });

            var result = await FilePicker.PickAsync(new PickOptions()
            {
                FileTypes = customFileType,
                PickerTitle = "Pick file"
            });
            if (result is null)
            {
                return;
            }

            List<Card> list = xml ? await serializer!.DeserializeXml<List<Card>>(result.FullPath) :
                await serializer!.DeserializeJson<List<Card>>(result.FullPath);

            foreach (var card in list)
            {
                var req = new AddCardCommand() { Card = card };
                await mediator.Send(req);
            }

            await UpdateCardList();
        }

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
