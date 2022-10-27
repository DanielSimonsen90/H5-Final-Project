#nullable enable

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private List<Connection> _connections = new();

        public ConnectionsViewModel(ContentPage page) : base(page)
        {
            GetUserConnections();
        }

        private async void GetUserConnections()
        {
            if (User is null) return;

            SimpleResponse res = await Client.Get(Endpoints.CONNECTIONS, User.Id.ToString());
            if (res.IsSuccess) _connections = res.GetContent<List<Connection>>() ?? new();
        }

        public void OnSwipe(object sender, SwipedEventArgs e)
        {
            Console.WriteLine();
        }
    }
}
