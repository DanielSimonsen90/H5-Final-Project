#nullable enable

namespace SmartWeightApp.ViewModels
{
    public class ConnectionViewModel : BaseViewModel
    {
        public ConnectionViewModel(Connection connection, bool canBeDeleted) : base()
        {
            Name = connection.Weight.Name ?? $"Weight{connection.Weight.Id}";
            CanBeDeleted = canBeDeleted;
            Connection = connection;

            ConnectCommand = new Command(OnConnect);
            DisconnectCommand = new Command(OnDisconnect);
            DeleteCommand = new Command(OnDelete);
        }

        private bool IsConnected => Connection.IsConnected;
        public string State => (IsConnected ? "Connected" : "Not Connected") + $" to {Name}";
        public Command PublicCommand => IsConnected ? DisconnectCommand : ConnectCommand;

        public string Name { get; }
        public bool CanBeDeleted { get; }
        public Connection Connection { get; }

        private Command ConnectCommand { get; set; }
        private Command DisconnectCommand { get; set; }
        public Command DeleteCommand { get; set; }
        
        private async void OnConnect()
        {
            throw new Exception("OnConnect doesn't work");

            if (User is null || !IsConnected) return;
            else if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Alert("Capturing not supported.", "The app is unable to connect to your camera.");
                return;
            }

            FileResult? result = await MediaPicker.Default.CapturePhotoAsync()
                // Temporarily default to QR code to weights/1
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
