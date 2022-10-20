
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartWeightApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        //public string Username { get; set; }
        //public string Password { get; set; }
        [ObservableProperty]
        private string _username = "";
        [ObservableProperty]
        private string _password = "";

        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new(OnLogin); 
        }
        
        private async void OnLogin()
        {
            SimpleResponse response = await Client.Post(Endpoints.LOGIN, new Login(Username, Password));
            
            try
            {
                User = response.GetContent<User>();
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Fejl", 
                    response.IsSuccess ? 
                        ex.Message : 
                        response.Message, 
                    "OK");
            }
        }
    }
}
