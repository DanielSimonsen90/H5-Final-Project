﻿#nullable enable

using System.Data.Entity.Core.Mapping;
using System.Runtime.CompilerServices;
using System.Web;

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private List<ConnectionViewModel> _connections = new();
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
            if (!res.IsSuccess) return;

            List<Connection> connections = res.GetContent<List<Connection>>() ?? new();
            Connections = connections.Select(conn => new ConnectionViewModel(conn, true)).ToList();
        }

        private async void OnNewConnection()
        {
            try
            {
                if (User is null) throw new AlertException("Invalid login state", "You must be logged in to add a connection!");
                if (!int.TryParse(WeightIdInput, out int weightId)) throw new AlertException("Invalid id", "Weight ids must be integers");
                else if (Connections.Any(connVm => connVm.Connection.WeightId == weightId)) throw new AlertException("Already added", $"Connection to weight {weightId} is already added.");

                SimpleResponse res = await Client.Post(Endpoints.CONNECTIONS, $"{User.Id}/{weightId}", new {});
                if (!res.IsSuccess) throw new AlertException("API Error", res.Message);

                Connection? conn = res.GetContent<Connection>();
                if (conn is null) throw new AlertException("Unable to get connection", "Connection received from API is null");

                // Add new connection and reset input
                Connections = new(Connections)
                {
                    new ConnectionViewModel(conn, true)
                };
                WeightIdInput = string.Empty;
            }
            catch (AlertException ex)
            {
                await Alert(ex.Title, ex.Message);
            }
        }
    }
}