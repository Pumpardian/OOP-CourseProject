using ACS_Reception.Application.DoctorUseCases.Commands;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.Domain.Abstractions;
using ACS_Reception.UI.Pages;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    public partial class DoctorsPageViewModel(IMediator mediator, ISerializer? serializer = null) : ObservableObject
    {
        private readonly IMediator mediator = mediator;
        private readonly ISerializer? serializer = serializer;
        private static readonly string[][] serializationFormats = [[".json"], [".xml"]];

        public ObservableCollection<Doctor> Doctors { get; set; } = [];

        [ObservableProperty] Doctor? selectedDoctor;
        [ObservableProperty] bool isSerializationPluginConnected = false;
        [ObservableProperty] List<string> sortBy = ["Default", "Last Name", "First Name", "Cabinet", "Type"];
        [ObservableProperty] bool reverseSort = false;

        [RelayCommand] async Task UpdateSerializationStatus() => await Task.Run(() => IsSerializationPluginConnected = serializer is not null);
        [RelayCommand] async Task SerializeJson() => await SerializeDoctors(false);
        [RelayCommand] async Task DeserializeJson() => await DeserializeDoctors(false);
        [RelayCommand] async Task SerializeXml() => await SerializeDoctors(true);
        [RelayCommand] async Task DeserializeXml() => await DeserializeDoctors(true);

        [RelayCommand] async Task UpdateDoctorList() => await GetDoctors();
        [RelayCommand] async Task ShowDoctorDetails(Doctor doctor) => await GoToDetailsPage(doctor);
        [RelayCommand] async Task AddDoctor() =>
            await GoToAddPage(new AddDoctorCommand() { Doctor = new Doctor() });

        [RelayCommand] async Task Sort(int index) => await SortCollection(index);

        private async Task SortCollection(int index)
        {
            var sorted = index switch
            {
                0 => [.. ReverseSort ? Doctors.OrderByDescending(c => c.Id) : Doctors.OrderBy(c => c.Id)],
                1 => [.. ReverseSort ? Doctors.OrderByDescending(c => c.LastName) : Doctors.OrderBy(c => c.LastName)],
                2 => [.. ReverseSort ? Doctors.OrderByDescending(c => c.FirstName) : Doctors.OrderBy(c => c.FirstName)],
                3 => [.. ReverseSort ? Doctors.OrderByDescending(c => c.CabinetNumber) : Doctors.OrderBy(c => c.CabinetNumber)],
                4 => [.. ReverseSort ? Doctors.OrderByDescending(c => ((int)c.DoctorType)) : Doctors.OrderBy(c => ((int)c.DoctorType))],
                _ => new List<Doctor>(),
            };


            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Doctors.Clear();

                foreach (var card in sorted)
                {
                    Doctors.Add(card);
                }
            });
        }

        private async Task SerializeDoctors(bool xml)
        {
            var result = await FolderPicker.PickAsync("");
            if (!result.IsSuccessful)
            {
                return;
            }

            var name = await Shell.Current.DisplayPromptAsync("Saving", "Enter Filename:", "OK", "Cancel", initialValue: "Doctors");
            name += xml ? ".xml" : ".json";

            name = Path.Combine(result.Folder.Path, name);
            if (xml)
            {
                await serializer!.SerializeXml(Doctors.ToList(), name);
            }
            else
            {
                await serializer!.SerializeJson(Doctors.ToList(), name);
            }
        }

        private async Task DeserializeDoctors(bool xml)
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

            List<Doctor> list = xml ? await serializer!.DeserializeXml<List<Doctor>>(result.FullPath) :
                await serializer!.DeserializeJson<List<Doctor>>(result.FullPath);

            foreach (var doc in list)
            {
                var req = new AddDoctorCommand() { Doctor = doc };
                await mediator.Send(req);
            }

            await UpdateDoctorList();
        }

        private async Task GoToDetailsPage(Doctor doctor)
        {
            IDictionary<string, object> parameters =
                new Dictionary<string, object>()
                {
                    { "Doctor", doctor }
                };

            await Shell.Current.GoToAsync(nameof(DoctorDetailsPage), parameters);
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
            var doctors = await mediator.Send(new GetDoctorsQuery());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Doctors.Clear();

                foreach (var doctor in doctors)
                {
                    Doctors.Add(doctor);
                }
            });
        }
    }
}
