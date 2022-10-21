using CommunityToolkit.Mvvm.ComponentModel;
using SmartWeightApp.Services;

namespace SmartWeightApp.ViewModels
{
    [ObservableObject]
    public partial class BaseViewModel
    {
        public BaseViewModel(ContentPage page)
        {
            Page = page;
            
            RefreshCommand = new(async () =>
            {
                IsRefreshing = true;
                await OnRefreshing();
                IsRefreshing = false;
            });
        }
        protected ContentPage Page { get; }

        private readonly DataStore<User> UserStore = DependencyService.Get<DataStore<User>>();
        protected User User
        {
            get => UserStore.Value;
            set => UserStore.Value = value;
        }

        protected ApiClient Client { get; set; } = new ApiClient();

        #region RefreshCommand
        public Command RefreshCommand { get; set; }

        [ObservableProperty]
        private bool isRefreshing = false;
        /// <summary>
        /// Override whenever you want to do something when the refresh command is called
        /// </summary>
        protected virtual async Task OnRefreshing() { await Task.CompletedTask; }
        #endregion

        protected static Task GoToAsync(ShellNavigationState state) => Shell.Current.GoToAsync(state);
        protected static Task Alert(string title, string message, string accept = "Okay", string cancel = null) => Shell.Current.DisplayAlert(title, message, accept, cancel);
    }
}
