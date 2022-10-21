namespace SmartWeightApp.Pages.Overview;

public partial class OverviewIndex : ContentPage
{
	public OverviewIndex()
	{
		InitializeComponent();
		BindingContext = new OverviewViewModel(this);
	}
}