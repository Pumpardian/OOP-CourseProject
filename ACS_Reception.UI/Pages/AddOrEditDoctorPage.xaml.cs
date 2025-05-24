using ACS_Reception.UI.ViewModels;
using System.Text.RegularExpressions;

namespace ACS_Reception.UI.Pages;

public partial class AddOrEditDoctorPage : ContentPage
{
	public AddOrEditDoctorPage(AddOrEditDoctorPageViewModel addOrEditDoctorPageViewModel)
	{
		BindingContext = addOrEditDoctorPageViewModel;
		InitializeComponent();
	}

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string regex = e.NewTextValue;
        if (String.IsNullOrEmpty(regex))
        {
            return;
        }

        if (!Regex.Match(regex, "^[0-9]+$").Success)
        {
            var entry = sender as Entry;
            entry!.Text = (string.IsNullOrEmpty(e.OldTextValue)) ? string.Empty : e.OldTextValue;
        }
    }
}