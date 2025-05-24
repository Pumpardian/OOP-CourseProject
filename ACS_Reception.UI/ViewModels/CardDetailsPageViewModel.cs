using ACS_Reception.Application.CardUseCases.Commands;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Application.RecordUseCases.Queries;
using ACS_Reception.Domain.Abstractions;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    [QueryProperty("Card", "Card")]
    public partial class CardDetailsPageViewModel(IMediator mediator, ISerializer serializer) : ObservableObject
    {
        private readonly IMediator mediator = mediator;
        private readonly ISerializer? serializer = serializer;
        private static readonly string[][] serializationFormats = [[".json"], [".xml"]];

        public ObservableCollection<CardRecord> Records { get; set; } = [];

        [ObservableProperty] Card card;
        [ObservableProperty] CardRecord selectedRecord;
        [ObservableProperty] bool isSerializationPluginConnected = false;
        [ObservableProperty] List<string> sortBy = ["Default", "Type", "Date", "Doctor"];
        [ObservableProperty] bool reverseSort = false;

        [RelayCommand] async Task UpdateSerializationStatus() => await Task.Run(() => IsSerializationPluginConnected = serializer is not null);
        [RelayCommand] async Task SerializeCardJson() => await SerializeCard(false, false);
        [RelayCommand] async Task SerializeRecordsJson() => await SerializeCard(false, true);
        [RelayCommand] async Task SerializeCardXml() => await SerializeCard(true, false);
        [RelayCommand] async Task SerializeRecordsXml() => await SerializeCard(true, true);

        [RelayCommand] async Task ShowRecordDetails(CardRecord record) => await GoToDetailsPage(record);
        [RelayCommand] async Task EditCard() => await GoToEditPage(new EditCardCommand() { Card = Card });
        [RelayCommand] async Task DeleteCard() => await RemoveCard(Card);

        [RelayCommand] async Task Sort(int index) => await SortCollection(index);

        [RelayCommand] async Task GetCardById()
        {
            Card = await mediator.Send(new GetCardByIdQuery(Card.Id));

            var records = await mediator.Send(new GetRecordsByCardIdQuery(Card.Id));
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Records.Clear();
                
                foreach (var rec in records)
                {
                    Records.Add(rec);
                }
            });
        }

        private async Task SortCollection(int index)
        {
            var sorted = index switch
            {
                0 => [.. ReverseSort ? Records.OrderByDescending(c => c.Id) : Records.OrderBy(c => c.Id)],
                1 => [.. ReverseSort ? Records.OrderByDescending(c => c.GetRecordName) : Records.OrderBy(c => c.GetRecordName)],
                2 => [.. ReverseSort ? Records.OrderByDescending(c => c.Date) : Records.OrderBy(c => c.Date)],
                3 => [.. ReverseSort ? Records.OrderByDescending(c => c.Doctor) : Records.OrderBy(c => c.Doctor)],
                _ => new List<CardRecord>(),
            };


            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Records.Clear();

                foreach (var card in sorted)
                {
                    Records.Add(card);
                }
            });
        }

        private async Task SerializeCard(bool xml, bool records)
        {
            var result = await FolderPicker.PickAsync("");
            if (!result.IsSuccessful)
            {
                return;
            }

            var name = await Shell.Current.DisplayPromptAsync("Saving", "Enter Filename:", "OK", "Cancel", initialValue: records ? "Records" : "Card");
            name += xml ? ".xml" : ".json";

            name = Path.Combine(result.Folder.Path, name);
            if (xml)
            {
                if (records)
                {
                    await serializer!.SerializeXml(Records, name);
                }
                else
                {
                    await serializer!.SerializeXml(Card, name);
                }
            }
            else
            {
                if (records)
                {
                    await serializer!.SerializeJson(Records, name);
                }
                else
                {
                    await serializer!.SerializeJson(Card, name);
                }
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
            var records = await mediator.Send(new GetRecordsByCardIdQuery(Card.Id));

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
