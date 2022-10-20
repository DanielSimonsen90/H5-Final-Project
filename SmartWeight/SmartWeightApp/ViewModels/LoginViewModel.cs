
namespace SmartWeightApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new(OnLogin); 
        }
        
        private async void OnLogin()
        {
            SimpleResponse response = await Client.Post(Endpoints.LOGIN, new Login(Username, Password));
            
            if (response.IsSuccess) await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            else await Shell.Current.DisplayAlert("Fejl", response.Message, "OK");
        }
    }
}
