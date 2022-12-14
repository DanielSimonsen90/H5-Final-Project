using SmartWeightApp.Pages.Connections;
#nullable enable

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionsViewModel : BaseViewModel<ConnectionsIndex>
    {
        [ObservableProperty]
        private List<ConnectionViewModel> _connections = new();
        [ObservableProperty]
        private string _weightIdInput = "";

        public Command NewConnectionCommand { get; }

        public ConnectionsViewModel(ConnectionsIndex content) : base(content)
        {
            
            NewConnectionCommand = new(OnNewConnection);
            GetUserConnections();
        }

        protected override Task OnRefreshing() => Task.Run(GetUserConnections);

        private async void GetUserConnections()
        {
            if (User is null) return;

            List<Connection> connections = await RequestUserConnections();
            Connections = connections.Select(conn => new ConnectionViewModel(conn, true)).ToList();
        }
        private async Task<List<Connection>> RequestUserConnections()
        {
            if (User is null) return new();

            SimpleResponse res = await Client.Get(Endpoints.CONNECTIONS, $"{User.Id}/all");
            return res.GetContent<List<Connection>>() ?? new();
        }

        private async void OnNewConnection()
        {
            try
            {
                if (User is null) throw new AlertException("Invalid login state", "You must be logged in to add a connection!");
                if (!int.TryParse(WeightIdInput, out int weightId)) throw new AlertException("Invalid id", "Weight ids must be integers");
                else if (Connections.Any(connVm => connVm.Connection.WeightId == weightId 
                    && connVm.Connection.IsConnected)) throw new AlertException(
                        "Already added", 
                        $"Connection to weight {weightId} is already added."
                   );

                SimpleResponse res = await Client.Post(Endpoints.CONNECTIONS, $"{User.Id}/{weightId}", new {});
                if (!res.IsSuccess) throw new AlertException("API Error", res.Message);

                Connection? conn = res.GetContent<Connection>();
                if (conn is null) throw new AlertException("Unable to get connection", "Connection received from API is null");

                ConnectionViewModel? existing = Connections.Find(connVm => connVm.Connection.Id == conn.Id);
                List<ConnectionViewModel> replacement = new(Connections);

                if (existing is not null) replacement.Remove(existing);
                replacement.Add(new ConnectionViewModel(conn, true));
                replacement.Reverse();

                // Add new connection and reset input
                Connections = replacement;
                WeightIdInput = string.Empty;
            }
            catch (AlertException ex)
            {
                await Alert(ex.Title, ex.Message);
            }
        }
    }
}
