
namespace SmartWeightApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _username = "";
        [ObservableProperty]
        private string _password = "";
        [ObservableProperty]
        private string _loginState;

        public Command LoginCommand { get; set; }
        public Command SignUpCommand { get; set; }

        public LoginViewModel(ContentPage page) : base(page)
        {
            _loginState = User is not null 
                ? $"Logged in as {User.Username}" 
                : "Please login to your accout.";

            LoginCommand = new(OnLogin);
            SignUpCommand = new(OnSignUp);
        }
        
        private async void OnLogin()
        {
            await Task.Run(() => _loginState = "Logging you in...");
            SimpleResponse response = await Client.Post(Endpoints.USERS_LOGIN, new Login(Username, Password));
            
            try
            {
                User = response.GetContent<User>();
                ResetStates();
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
            SimpleResponse loginAttempt = await Client.Post(Endpoints.USERS_LOGIN, login);

            if (loginAttempt.IsSuccess)
            {
                bool shouldLogin = await Alert("User already exists", "Would you like to login to this user?", "Yes", "No");
                if (shouldLogin) OnLogin();
                return;
            }

            var user = new User(Username, Password);
            SimpleResponse userRes = await Client.Post(Endpoints.USERS, user);

            if (!userRes.IsSuccess) await Alert("Fejl", userRes.Message);
            else OnLogin();
        }

        private void ResetStates()
        {
            _username = string.Empty;
            _password = string.Empty;
            _loginState = User is not null ? $"Logged in as {User.Username}" : "Please login to your accout.";
        }
    }
}
