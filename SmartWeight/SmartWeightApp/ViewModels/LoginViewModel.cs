
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
        public Command SignUpCommand { get; set; }

        public LoginViewModel(ContentPage page) : base(page)
        {
            LoginCommand = new(OnLogin);
            SignUpCommand = new(OnSignUp);
        }
        
        private async void OnLogin()
        {
            SimpleResponse response = await Client.Post(Endpoints.USERS_LOGIN, new Login(Username, Password));
            
            try
            {
                User = response.GetContent<User>();
                ResetEditors();
                await GoToAsync($"//{nameof(MainPage)}");
            }
            catch (Exception ex)
            {
                await Alert("Fejl", 
                    response.IsSuccess ? 
                        ex.Message : 
                        response.Message
                    );
            }
        }
        private async void OnSignUp()
        {
            var login = new Login(Username, Password);
            SimpleResponse loginAttempt = await Client.Post(Endpoints.USERS, login);

            if (loginAttempt.IsSuccess)
            {
                bool shouldLogin = await Alert("Bruger findes allerde", "Vil du logge ind med denne bruger?", "Ja", "Nej");
                if (shouldLogin) OnLogin();
                return;
            }

            var user = new User(Username, Password);
            SimpleResponse userRes = await Client.Post(Endpoints.USERS, user);

            if (!userRes.IsSuccess) await Alert("Fejl", userRes.Message);
            else OnLogin();
        }

        private void ResetEditors()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
    }
}
