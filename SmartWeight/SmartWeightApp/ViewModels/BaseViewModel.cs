using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartWeightApp.ViewModels
{
    [ObservableObject]
    public partial class BaseViewModel
    {
        public BaseViewModel()
        {
            RefreshCommand = new(async () =>
            {
                IsRefreshing = true;
                await OnRefreshing();
                IsRefreshing = false;
            });
        }

        protected User User { get; set; }
        protected ApiClient Client { get; set; }

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
    }
}
