namespace SmartWeightApp.Pages.Login;

public partial class LoginIndex : ContentPage
{
	public LoginIndex()
	{
		InitializeComponent();
		BindingContext = new LoginViewModel();
	}
}