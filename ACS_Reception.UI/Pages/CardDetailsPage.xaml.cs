using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class CardDetailsPage : ContentPage
{
	public CardDetailsPage(CardDetailsPageViewModel cardDetailsPageViewModel)
	{
		BindingContext = cardDetailsPageViewModel;
		InitializeComponent();
	}
}