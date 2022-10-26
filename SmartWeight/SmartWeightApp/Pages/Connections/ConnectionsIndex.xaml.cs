using SmartWeightApp.ViewModels;

namespace SmartWeightApp.Pages.Connections;

public partial class ConnectionsIndex : ContentPage
{
	public ConnectionsIndex()
	{
		InitializeComponent();
		BindingContext = new ConnectionsViewModel(this);
	}
}