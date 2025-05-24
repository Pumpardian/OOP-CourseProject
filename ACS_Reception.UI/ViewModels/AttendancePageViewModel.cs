using ACS_Reception.Application;
using ACS_Reception.Application.CardUseCases.Queries;
using ACS_Reception.Application.DoctorUseCases.Queries;
using ACS_Reception.Domain.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ACS_Reception.UI.ViewModels
{
    public partial class AttendancePageViewModel(IMediator mediator, AttendanceDistributor attendanceDistributor)
        : ObservableObject
    {
        private readonly IMediator mediator = mediator;
        private readonly AttendanceDistributor attendanceDistributor = attendanceDistributor;
        public List<AttendanceReason> AttendanceReasons { get; set; } = [.. Enum.GetValues<AttendanceReason>()];

        public ObservableCollection<Card> Cards { get; set; } = [];
        public ObservableCollection<Doctor> DoctorsToAttend { get; set; } = [];

        [ObservableProperty] AttendanceInfo attendanceInfo = new();

        [RelayCommand] async Task UpdateCardList() => await GetCards();
        [RelayCommand] async Task UpdateDoctorList() =>
            await GetDoctors(attendanceDistributor.ForwardToDoctor(AttendanceInfo.AttendanceReason!.Value));
        [RelayCommand] async Task Attend() => await AttendToDoctor();

        public async Task AttendToDoctor()
        {
            string response = await attendanceDistributor.ForwardToStrategy(AttendanceInfo);

            await Shell.Current.DisplayAlert("Doctor Response", response, "OK");
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

        public async Task GetDoctors(DoctorType doctorType)
        {
            var doctors = await mediator.Send(new GetDoctorsQuery());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                DoctorsToAttend.Clear();

                foreach (var doctor in doctors)
                {
                    if (doctor.DoctorType == doctorType)
                    DoctorsToAttend.Add(doctor);
                }
            });
        }
    }
}
