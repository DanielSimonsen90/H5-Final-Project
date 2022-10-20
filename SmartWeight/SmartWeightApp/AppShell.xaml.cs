using SmartWeightApp.Pages.Connections;
using SmartWeightApp.Pages.Login;
using SmartWeightApp.Pages.Overview;

namespace SmartWeightApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginIndex), typeof(LoginIndex));
            Routing.RegisterRoute(nameof(ConnectionsIndex), typeof(ConnectionsIndex));
            Routing.RegisterRoute(nameof(OverviewIndex), typeof(OverviewIndex));
        }
    }
}