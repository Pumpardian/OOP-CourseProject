using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class AddOrEditCardPage : ContentPage
{
	public AddOrEditCardPage(AddOrEditCardPageViewModel addOrEditCardPageViewModel)
	{
		BindingContext = addOrEditCardPageViewModel;
		InitializeComponent();
	}
}