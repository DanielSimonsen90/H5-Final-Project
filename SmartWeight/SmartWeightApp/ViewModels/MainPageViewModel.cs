namespace SmartWeightApp.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel<MainPage>
    {
        public string WelcomeText => User is null ?
            "Please login before using SmartWeight." :
            $"Welcome to SmartWeight, {User.Username}!";
        //public string WelcomeText => $"Welcome to {Application.Current.Resources["AppName"]}, {User?.Username}!";

        public MainPageViewModel(MainPage page) : base(page) {}
    }
}
