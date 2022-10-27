using SmartWeightApp.Services;

namespace SmartWeightApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            DependencyService.RegisterSingleton(new DataStore<User>(null, StorageKeys.USER, value => 
                value is not null // value exists
                && value.Id > -1 // value has a vaild userId
                && !string.IsNullOrEmpty(value.Username)) // value has a Username
             );

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