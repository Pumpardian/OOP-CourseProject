using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class AddOrEditDoctorPage : ContentPage
{
	public AddOrEditDoctorPage(AddOrEditDoctorPageViewModel addOrEditDoctorPageViewModel)
	{
		BindingContext = addOrEditDoctorPageViewModel;
		InitializeComponent();
	}
}