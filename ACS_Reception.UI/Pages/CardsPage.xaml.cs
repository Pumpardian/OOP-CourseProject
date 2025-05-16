using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class CardsPage : ContentPage
{
	public CardsPage(CardsPageViewModel cardsPageViewModel)
	{
		BindingContext = cardsPageViewModel;
		InitializeComponent();
	}
}