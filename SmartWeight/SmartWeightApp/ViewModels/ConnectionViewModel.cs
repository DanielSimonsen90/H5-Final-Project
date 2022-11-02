using SmartWeightApp.Pages.Connections;
#nullable enable

namespace SmartWeightApp.ViewModels
{
    public partial class ConnectionViewModel : BaseViewModel<ConnectionsIndex>
    {
        public ConnectionViewModel(Connection connection, bool canBeDeleted) : base()
        {
            _name = connection.Weight.Name ?? $"Weight{connection.WeightId}";
            CanBeDeleted = canBeDeleted;
            _connection = connection;

            ConnectCommand = new Command(OnConnect);
            DisconnectCommand = new Command(OnDisconnect);
            DeleteCommand = new Command(OnDelete);

            PropertyChanged += OnConnectionChanged;
        }

        private void OnConnectionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Connection) && ConnectionsStore.Value is not null)
            {
                ConnectionsStore.Value = ConnectionsStore.Value
                    .Select(conn => conn.Id != Connection.Id ? conn : Connection)
                    .ToList();
            }
        }

        private bool IsConnected => Connection.IsConnected;
        public string State => (IsConnected ? "Connected" : "Not connected") + $" to {_name}";
        public bool CanBeDeleted { get; }


        private readonly string _name;
        [ObservableProperty]
        private Connection _connection;

        public Command PublicCommand => IsConnected ? DisconnectCommand : ConnectCommand;
        private Command ConnectCommand { get; set; }
        private Command DisconnectCommand { get; set; }
        public Command DeleteCommand { get; set; }

        //private async void OnConnect()
        //{
        //    if (Connection is null 
        //        || User is null
        //        || IsConnected) return;

        //    SimpleResponse res = await Client.Post(Endpoints.CONNECTIONS, $"{User.Id}/{Connection.WeightId}", new {});
        //    if (!res.IsSuccess)
        //    {
        //        await Alert($"Unable to connect to {_name}.", res.Message);
        //        return;
        //    }

        //    Connection = res.GetContent<Connection>() ?? Connection;
        //}
        private async void OnConnect()
        {
            if (User is null || IsConnected) return;
            else if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Alert("Capturing not supported.", "The app is unable to connect to your camera.");
                return;
            }

            PermissionStatus permission = await Permissions.CheckStatusAsync<Permissions.Camera>();
            bool request = true;
            do
            {
                if (permission != PermissionStatus.Granted)
                {
                    if (permission is PermissionStatus.Denied 
                                   or PermissionStatus.Disabled)
                    {
                        request = await Alert(
                            "Camera access denied", 
                            "To connect to your weight, the app must use your camera to scan the QR code.", 
                            "Connect", "Cancel"
                        );
                    }
                    if (request) permission = await Permissions.RequestAsync<Permissions.Camera>();
                    else return;
                }
            } while (request && permission != PermissionStatus.Granted);

            FileResult? result = await MediaPicker.Default.CapturePhotoAsync()
                // TODO: Remove temporary QR code to weights/1
                ?? new FileResult("https://media.discordapp.net/attachments/777577204775125102/1034446105649365052/unknown.png");
            if (result is null)
            {
                await Alert("No camera permission", "The camera could not be opened.");
                return;
            }

            using var stream = await result.OpenReadAsync();
            ImageSource image = ImageSource.FromStream(() => stream);
            Console.WriteLine(image);
        }
        
        private async void OnDisconnect()
        {
            if (User is null || !IsConnected) return;

            SimpleResponse res = await Client.Delete(Endpoints.CONNECTIONS, $"{User.Id}?fromApp=true");
            if (res.IsSuccess) await Alert("Disconnected", $"You are no longer connected to any weight.");
            else await Alert("Unable to disconnect", res.Message);
        }
        private async void OnDelete()
        {
            if (!CanBeDeleted || User is null) return;

            SimpleResponse res = await Client.Delete(Endpoints.CONNECTIONS, $"{User.Id}?fromApp=true&delete=true");
            if (!res.IsSuccess) await Alert("Unable to delete connection", res.Message);

            //TODO: Update UI to delete self
        }
    }
}
