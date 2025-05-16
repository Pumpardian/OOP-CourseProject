using ACS_Reception.Application.DoctorUseCases.Commands;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    public partial class DoctorsPageViewModel(IMediator mediator) : ObservableObject
    {
        private readonly IMediator mediator = mediator;

        public ObservableCollection<Doctor> Doctors { get; set; } = [];

        [ObservableProperty] Doctor? selectedDoctor;

        [RelayCommand] async Task UpdateDoctorList() => await GetDoctors();
        [RelayCommand] async Task ShowCardDetails(Doctor doctor) => await GoToDetailsPage(doctor);
        [RelayCommand]
        async Task AddDoctor() =>
            await GoToAddPage(new AddDoctorCommand() { Doctor = new Doctor("First Name", "Last Name", 0, DoctorType.GeneralPractitioner) });

        private async Task GoToDetailsPage(Doctor doctor)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Doctor", doctor }
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

            await Shell.Current.GoToAsync(nameof(AddOrEditDoctorPage), parameters);
        }

        public async Task GetDoctors()
        {
            var cards = await mediator.Send(new GetDoctorsQuery());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Doctors.Clear();

                foreach (var card in cards)
                {
                    Doctors.Add(card);
                }
            });
        }
    }
}
