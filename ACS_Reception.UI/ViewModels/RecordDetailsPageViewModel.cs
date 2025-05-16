using ACS_Reception.Application.RecordUseCases.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    [QueryProperty("CardRecord", "CardRecord")]
    public partial class RecordDetailsPageViewModel(IMediator mediator) : ObservableObject
    {
        private readonly IMediator mediator = mediator;

        [ObservableProperty] CardRecord record;

        [RelayCommand]
        async Task GetRecordById()
        {
            Record = await mediator.Send(new GetRecordByIDQuery(Record.Id));

            if (Record is null)
            {
                return;
            }
        }
    }
}
