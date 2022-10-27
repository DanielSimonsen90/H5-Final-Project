using SmartWeightApp.ViewModels;

namespace SmartWeightApp.Pages.Connections;

public partial class ConnectionsIndex : ContentPage
{
    private ConnectionsViewModel _viewModel;
    public ConnectionsIndex()
	{
		InitializeComponent();
		_viewModel = new ConnectionsViewModel(this);
        BindingContext = _viewModel;
    }

	private void OnSwipe(object sender, SwipedEventArgs e) => _viewModel.OnSwipe(sender, e);
}