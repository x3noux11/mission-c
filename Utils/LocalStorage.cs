using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace mission.Utils
{
    public class LocalStorage
    {
        private readonly string _storagePath;

        public LocalStorage()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _storagePath = Path.Combine(appDataPath, "mission-c", "data");
            Directory.CreateDirectory(_storagePath);
        }

        public async Task<T?> LoadAsync<T>(string key)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, $"{key}.json");
                if (File.Exists(filePath))
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch
            {
                // En cas d'erreur, on retourne null
            }
            return default;
        }

        public async Task SaveAsync<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, $"{key}.json");
                var json = JsonSerializer.Serialize(value);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch
            {
                // En cas d'erreur, on ignore
            }
        }
    }
} 