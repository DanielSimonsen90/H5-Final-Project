using System.Text.Json;
#nullable enable

namespace SmartWeightApp.Models
{
    public class LocalStorage
    {
        public static async Task<T?> Get<T>(StorageKeys key)
        {
            string json = await SecureStorage.GetAsync(GetName(key));
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }
        public static async void Set<T>(StorageKeys key, T? value, bool removeIfNull = false)
        {
            if (value is not null) await SecureStorage.SetAsync(GetName(key), JsonSerializer.Serialize(value));
            else if (removeIfNull && value is null) Remove(key);
        }
        public static bool Remove(StorageKeys key) => SecureStorage.Remove(GetName(key));

        private static string GetName(StorageKeys key) => Enum.GetName(typeof(StorageKeys), key)!;
    }
}
