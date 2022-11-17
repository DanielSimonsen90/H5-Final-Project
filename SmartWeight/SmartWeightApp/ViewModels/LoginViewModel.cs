using SmartWeightApp.Pages.Login;
using SmartWeightApp.Pages.Connections;

namespace SmartWeightApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel<LoginIndex>
    {
        [ObservableProperty]
        private string _username = "";
        [ObservableProperty]
        private string _password = "";
        [ObservableProperty]
        private string _loginState;

        public Command LoginCommand { get; set; }
        public Command SignUpCommand { get; set; }

        public LoginViewModel(LoginIndex content) : base(content)
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
                await GoToAsync(nameof(ConnectionsIndex));
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

            if (userRes.IsSuccess) OnLogin();
            else await Alert("Fejl", userRes.Message);
        }

        private void ResetStates()
        {
            Username = string.Empty;
            Password = string.Empty;
            LoginState = User is not null
                ? $"Logged in as {User.Username}"
                : "Please login to your accout.";
        }
    }
}
