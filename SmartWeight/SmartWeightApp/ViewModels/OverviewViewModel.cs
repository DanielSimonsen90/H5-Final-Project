using CommunityToolkit.Mvvm.ComponentModel;
using SmartWeightApp.Pages.Overview;

namespace SmartWeightApp.ViewModels
{
    public partial class OverviewViewModel : BaseViewModel<OverviewIndex>
    {
        [ObservableProperty]
        private ICollection<Measurement> _measurements = new List<Measurement>();

        public OverviewViewModel(OverviewIndex content) : base(content) 
        {
            IsRefreshing = true; // Load measurements on page load
            LoadMeasurementsCommand = new(() => { IsRefreshing = true; });

            PropertyChanged += OnMeasurementsChanged;
        }

        private void OnMeasurementsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Measurements))
            {
                MeasurementsStore.Value = Measurements.ToList();
            }
        }

        public Command LoadMeasurementsCommand { get; set; }

        protected override async Task OnRefreshing()
        {
            if (User is null)
            {
                await Alert("Invalid login state", "You must login before using the Overview page.");
                return;
            }
            
            SimpleResponse response = await Client.Get(Endpoints.MEASUREMENTS_OVERVIEW, User.Id.ToString());
            if (response.IsSuccess) Measurements = response.GetContent<ICollection<Measurement>>();
            else await Alert("Error", response.Message);
        }
    }
}
