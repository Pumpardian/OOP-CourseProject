using ACS_Reception.Application.DoctorUseCases.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    public partial class AddOrEditDoctorPageViewModel(IMediator mediator) : ObservableObject, IQueryAttributable
    {
        private readonly IMediator mediator = mediator;
        public List<DoctorType> DoctorTypes { get; set; } = Enum.GetValues<DoctorType>().ToList();

        [ObservableProperty] IAddOrEditDoctorRequest request;

        [RelayCommand]
        async Task AddOrEditDoctor()
        {
            await mediator.Send(Request);
            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Request = (query["Request"] as IAddOrEditDoctorRequest)!;
        }
    }
}
