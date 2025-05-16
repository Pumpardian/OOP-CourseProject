using ACS_Reception.Application.CardUseCases.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    public partial class AddOrEditCardPageViewModel(IMediator mediator) : ObservableObject, IQueryAttributable
    {
        private readonly IMediator mediator = mediator;

        [ObservableProperty] IAddOrEditCardRequest request;

        [RelayCommand]
        async Task AddOrEditCard()
        {
            await mediator.Send(Request);
            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Request = (query["Request"] as IAddOrEditCardRequest)!;
        }
    }
}
