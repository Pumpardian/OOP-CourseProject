using ACS_Reception.Application.DoctorUseCases.Commands;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    [QueryProperty("Doctor", "Doctor")]
    public partial class DoctorDetailsPageViewModel(IMediator mediator) : ObservableObject
    {
        private readonly IMediator mediator = mediator;

        [ObservableProperty] Doctor doctor;

        [RelayCommand] async Task EditDoctor() => await GoToEditPage(new EditDoctorCommand() { Doctor = Doctor });
        [RelayCommand] async Task DeleteDoctor() => await RemoveCard(Doctor);

        [RelayCommand]
        async Task GetDoctorById()
        {
            Doctor = await mediator.Send(new GetDoctorByIdQuery(Doctor.Id));

            if (Doctor is null)
            {
                return;
            }
        }

        private async Task RemoveCard(Doctor doctor)
        {
            await mediator.Send(new DeleteDoctorCommand(doctor));
            await Shell.Current.GoToAsync("..");
        }

        private async Task GoToEditPage(IAddOrEditDoctorRequest request)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Request", request }
                };

            await Shell.Current.GoToAsync(nameof(AddOrEditDoctorPage), parameters);
        }
    }
}
