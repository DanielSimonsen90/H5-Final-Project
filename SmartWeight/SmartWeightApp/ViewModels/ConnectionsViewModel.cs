#nullable enable

using System.Data.Entity.Core.Mapping;

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private List<Connection> _connections = new();
        [ObservableProperty]
        private string _weightIdInput = "";

        public Command NewConnectionCommand { get; }

        public ConnectionsViewModel(ContentPage page) : base(page)
        {
            NewConnectionCommand = new(OnNewConnection);

            GetUserConnections();
        }

        private async void GetUserConnections()
        {
            if (User is null) return;

            SimpleResponse res = await Client.Get(Endpoints.CONNECTIONS, $"{User.Id}?fromApp=true");
            if (res.IsSuccess) _connections = res.GetContent<List<Connection>>() ?? new();
        }

        private async void OnNewConnection()
        {
            try
            {
                if (User is null) throw new AlertException("Invalid login state", "You must be logged in to add a connection!");
                if (int.TryParse(WeightIdInput, out int weightId)) throw new AlertException("Invalid id", "Weight ids must be integers");

                SimpleResponse res = await Client.Post(Endpoints.CONNECTIONS, $"{User.Id}/{weightId}");
                if (!res.IsSuccess) throw new AlertException("API Error", res.Message);

                Connection? conn = res.GetContent<Connection>();
                if (conn is null) throw new AlertException("Unable to get connection", "Connection received from API is null");

                _connections.Add(conn);
            }
            catch (AlertException ex)
            {
                await Alert(ex.Title, ex.Message);
            }
        }

        public void OnSwipe(object sender, SwipedEventArgs e)
        {
            Console.WriteLine();
        }
    }
}
