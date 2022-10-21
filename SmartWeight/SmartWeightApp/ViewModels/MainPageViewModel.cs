using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;
using SmartWeightApp.Pages.Login;
using SmartWeightApp.Pages.Overview;

namespace SmartWeightApp.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        public string WelcomeText => User is null ?
            "Please login before using SmartWeight." :
            $"Welcome to SmartWeight, {User.Username}!";
        //public string WelcomeText => $"Welcome to {Application.Current.Resources["AppName"]}, {User?.Username}!";

        public MainPageViewModel(ContentPage page) : base(page)
        {
            
        }
    }
}
