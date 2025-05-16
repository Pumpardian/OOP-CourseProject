using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class DoctorDetailsPage : ContentPage
{
	public DoctorDetailsPage(DoctorDetailsPageViewModel doctorDetailsPageViewModel)
	{
		BindingContext = doctorDetailsPageViewModel;
		InitializeComponent();
	}
}