using ACS_Reception.UI.Pages;

namespace ACS_Reception.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddOrEditCardPage),
                typeof(AddOrEditCardPage));
            Routing.RegisterRoute(nameof(AddOrEditDoctorPage),
                typeof(AddOrEditDoctorPage));
            Routing.RegisterRoute(nameof(AttendancePage),
                typeof(AttendancePage));
            Routing.RegisterRoute(nameof(CardDetailsPage),
                typeof(CardDetailsPage));
            Routing.RegisterRoute(nameof(CardsPage),
                typeof(CardsPage));
            Routing.RegisterRoute(nameof(DoctorDetailsPage),
                typeof(DoctorDetailsPage));
            Routing.RegisterRoute(nameof(DoctorsPage),
                typeof(DoctorsPage));
            Routing.RegisterRoute(nameof(RecordDetailsPage),
                typeof(RecordDetailsPage));
            Routing.RegisterRoute(nameof(ChartsPage),
                typeof(ChartsPage));
        }
    }
}
