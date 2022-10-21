using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _connectionText = "Not Connected";

        public Command ConnectCommand { get; set; }
        public Command DisconnectCommand { get; set; }

        public ConnectionViewModel(ContentPage page) : base(page)
        {
            ConnectCommand = new Command(OnConnect);
            DisconnectCommand = new Command(OnDisconnect);

            if (User is not null) IsUserConnected();
        }

        private async void IsUserConnected()
        {
            SimpleResponse res = await Client.Get(Endpoints.CONNECTIONS, User.Id.ToString());

            if (!res.IsSuccess) return;
            
            Connection conn = res.GetContent<Connection>();
            if (conn is null) return;

            res = await Client.Get(Endpoints.WEIGHTS, conn.WeightId.ToString());
            if (!res.IsSuccess) return;

            Weight weight = res.GetContent<Weight>();

            _connectionText = $"Connected to {weight.Name}.";
        }


        private void OnConnect()
        {
            throw new NotImplementedException("Camera function not implemented yet.");
        }
        private async void OnDisconnect()
        {
            SimpleResponse res = await Client.Delete(Endpoints.CONNECTIONS, $"{User.Id}?fromApp=true");
            if (res.IsSuccess)
            {
                _connectionText = "Not Connected.";
                await Alert("Disconnected", $"You are no longer connected to any weight.");
            }
            else await Alert("Unable to disconnect", res.Message);
        }
    }
}
