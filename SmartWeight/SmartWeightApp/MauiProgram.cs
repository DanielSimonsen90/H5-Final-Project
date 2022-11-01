using SmartWeightApp.Services;

namespace SmartWeightApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            LocalStorage.Remove(StorageKeys.CONNECTION);

            // Cache crucial models
            DependencyService.RegisterSingleton(new DataStore<User>(null, StorageKeys.USER, value => 
                value is not null // value exists
                && value.Id > -1 // value has a vaild userId
                && !string.IsNullOrEmpty(value.Username)) // value has a Username
             );
            DependencyService.RegisterSingleton(new DataStore<List<Measurement>>(new List<Measurement>(), StorageKeys.MEASUREMENTS));
            DependencyService.RegisterSingleton(new DataStore<List<Connection>>(new List<Connection>(), StorageKeys.CONNECTION));

            return MauiApp.CreateBuilder()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).Build();
        }
    }
}