using ACS_Reception.Application.DoctorUseCases.Commands;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.Domain.Abstractions;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ACS_Reception.UI.ViewModels
{
    [QueryProperty("Doctor", "Doctor")]
    public partial class DoctorDetailsPageViewModel(IMediator mediator, ISerializer? serializer = null) : ObservableObject
    {
        private readonly IMediator mediator = mediator;
        private readonly ISerializer? serializer = serializer;

        [ObservableProperty] Doctor doctor;
        [ObservableProperty] bool isSerializationPluginConnected = false;

        [RelayCommand] async Task UpdateSerializationStatus() => await Task.Run(() => IsSerializationPluginConnected = serializer is not null);
        [RelayCommand] async Task SerializeJson() => await SerializeDoctor(false);
        [RelayCommand] async Task SerializeXml() => await SerializeDoctor(true);
        [RelayCommand] async Task EditDoctor() => await GoToEditPage(new EditDoctorCommand() { Doctor = Doctor });
        [RelayCommand] async Task DeleteDoctor() => await RemoveDoctor(Doctor);

        [RelayCommand] async Task GetDoctorById() => Doctor = await mediator.Send(new GetDoctorByIdQuery(Doctor.Id));

        private async Task SerializeDoctor(bool xml)
        {
            var result = await FolderPicker.PickAsync("");
            if (!result.IsSuccessful)
            {
                return;
            }

            var name = await Shell.Current.DisplayPromptAsync("Saving", "Enter Filename:", "OK", "Cancel", initialValue: "Doctor");
            name += xml ? ".xml" : ".json";

            name = Path.Combine(result.Folder.Path, name);
            if (xml)
            {
                await serializer!.SerializeXml(Doctor, name);
            }
            else
            {
                await serializer!.SerializeJson(Doctor, name);
            }
        }

        private async Task RemoveDoctor(Doctor doctor)
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
