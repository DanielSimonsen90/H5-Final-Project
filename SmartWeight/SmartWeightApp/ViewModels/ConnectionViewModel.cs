#nullable enable

using System.Text;

namespace SmartWeightApp.ViewModels
{
    internal partial class ConnectionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _connectionText = "Not Connected";
        public bool IsConnected => ConnectionText == "Not Connected";

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
            
            Connection? conn = res.GetContent<Connection>();
            if (conn is null) return;

            res = await Client.Get(Endpoints.WEIGHTS, conn.WeightId.ToString());
            if (!res.IsSuccess) return;

            Weight? weight = res.GetContent<Weight>();
            if (weight is null) return;

            _connectionText = $"Connected to {weight.Name}.";
        }


        private async void OnConnect()
        {
            if (!MediaPicker.Default.IsCaptureSupported)
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
            var image = ImageSource.FromStream(() => stream);
            Console.WriteLine(image);
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
