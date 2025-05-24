using ACS_Reception.Application.RecordUseCases.Commands;
using ACS_Reception.Domain.Abstractions;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    public partial class RecordDetailsPageViewModel(IMediator mediator, ISerializer serializer) : ObservableObject, IQueryAttributable
    {
        private readonly IMediator mediator = mediator;
        private readonly ISerializer? serializer = serializer;

        [ObservableProperty] CardRecord cardRecord;
        [ObservableProperty] bool isSerializationPluginConnected = false;

        [RelayCommand] async Task UpdateSerializationStatus() => await Task.Run(() => IsSerializationPluginConnected = serializer is not null);
        [RelayCommand] async Task SerializeJson() => await SerializeRecord(false);
        [RelayCommand] async Task SerializeXml() => await SerializeRecord(true);

        [RelayCommand] async Task DeleteRecord() => await RemoveRecord(CardRecord);

        private async Task SerializeRecord(bool xml)
        {
            var result = await FolderPicker.PickAsync("");
            if (!result.IsSuccessful)
            {
                return;
            }

            var name = await Shell.Current.DisplayPromptAsync("Saving", "Enter Filename:", "OK", "Cancel", initialValue: "Record");
            name += xml ? ".xml" : ".json";

            name = Path.Combine(result.Folder.Path, name);
            if (xml)
            {
                await serializer!.SerializeXml(CardRecord, name);
            }
            else
            {
                await serializer!.SerializeJson(CardRecord, name);
            }
        }

        private async Task RemoveRecord(CardRecord record)
        {
            await mediator.Send(new DeleteRecordCommand(record));
            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CardRecord = (query["CardRecord"] as CardRecord)!;
        }
    }
}
