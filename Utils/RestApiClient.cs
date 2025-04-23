using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace mission.Utils
{
    /// <summary>
    /// Client pour communiquer avec l'API REST
    /// </summary>
    public class RestApiClient : IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initialise une nouvelle instance de la classe RestApiClient
        /// </summary>
        public RestApiClient(string baseUrl = "http://172.29.102.10/Api_Rest/")
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Effectue une requête GET et retourne les données désérialisées
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur HTTP GET sur {endpoint}: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur de désérialisation JSON sur GET {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Effectue une requête POST et retourne les données désérialisées
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, T data)
        {
            try
            {
                string jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur HTTP POST sur {endpoint}: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur de sérialisation/désérialisation JSON sur POST {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Effectue une requête PUT et retourne les données désérialisées
        /// </summary>
        public async Task PutAsync<T>(string endpoint, T data)
        {
            try
            {
                string jsonPayload = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur HTTP PUT sur {endpoint}: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur de sérialisation JSON sur PUT {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Effectue une requête DELETE et retourne un statut de succès
        /// </summary>
        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur HTTP DELETE sur {endpoint}: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
} 