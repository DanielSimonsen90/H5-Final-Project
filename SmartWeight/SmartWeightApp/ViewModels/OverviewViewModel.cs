using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartWeightApp.ViewModels
{
    public partial class OverviewViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ICollection<Measurement> _measurements = new List<Measurement>();

        [ObservableProperty]
        private string _noMeasurements = "No measurements yet.";

        public OverviewViewModel(ContentPage page) : base(page)
        {
        }

        protected override async Task OnRefreshing()
        {
            if (User is null) return;
            
            SimpleResponse response = await Client.Get(Endpoints.MEASUREMENTS_OVERVIEW, User.Id.ToString());
            if (response.IsSuccess) _measurements = response.GetContent<ICollection<Measurement>>();
            else await Alert("Error", response.Message, "OK");
        }
    }
}
