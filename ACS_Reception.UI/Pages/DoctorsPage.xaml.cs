using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class DoctorsPage : ContentPage
{
	public DoctorsPage(DoctorsPageViewModel doctorsPageViewModel)
	{
		BindingContext = doctorsPageViewModel;
		InitializeComponent();
	}
}