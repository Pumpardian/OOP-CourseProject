using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class AttendancePage : ContentPage
{
	public AttendancePage(AttendancePageViewModel attendancePageViewModel)
	{
		BindingContext = attendancePageViewModel;
		InitializeComponent();
	}
}