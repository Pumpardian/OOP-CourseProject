using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI.Pages;

public partial class RecordDetailsPage : ContentPage
{
	public RecordDetailsPage(RecordDetailsPageViewModel recordDetailsPageView)
	{
		BindingContext = recordDetailsPageView;
		InitializeComponent();
	}
}