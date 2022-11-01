using SmartWeightApp.Services;
using System.Runtime.CompilerServices;
#nullable enable

namespace SmartWeightApp.ViewModels
{
    [ObservableObject]
    public partial class BaseViewModel<TContent> where TContent : TemplatedPage
    {
        protected BaseViewModel()
        {
            RefreshCommand = new(async () =>
            {
                IsRefreshing = true;
                await OnRefreshing();
                IsRefreshing = false;
            });
        }
        public BaseViewModel(TContent? content) : this() => Content = content;
        protected TContent? Content { get; }

        #region Store Providers
        protected readonly DataStore<User> UserStore = DependencyService.Get<DataStore<User>>();
        protected readonly DataStore<List<Measurement>> MeasurementsStore = DependencyService.Get<DataStore<List<Measurement>>>();
        protected readonly DataStore<List<Connection>> ConnectionsStore = DependencyService.Get<DataStore<List<Connection>>>();
        protected User? User
        {
            get => UserStore.Value;
            set => OnUserChanged(UserStore.Value = value);
        }
        #endregion

        private async void OnUserChanged(User? user)
        {
            if (user is null) return;
            string userId = user.Id.ToString();

            SimpleResponse measurementsRes = await Client.Get(Endpoints.MEASUREMENTS_OVERVIEW, userId);
            SimpleResponse connectionsRes = await Client.Get(Endpoints.CONNECTIONS, userId);

            if (measurementsRes.IsSuccess) MeasurementsStore.Value = measurementsRes.GetContent<List<Measurement>>() ?? new();
            if (connectionsRes.IsSuccess) ConnectionsStore.Value = connectionsRes.GetContent<List<Connection>>() ?? new();
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

        protected static Task Alert(string title, string message, string accept = "Okay") => Shell.Current.DisplayAlert(title, message, accept);
        protected static Task<bool> Alert(string title, string message, string accept, string cancel) => Shell.Current.DisplayAlert(title, message, accept, cancel);
    }
}
