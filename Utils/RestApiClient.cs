using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private bool _isConnected = false;
        private readonly System.Timers.Timer _connectionCheckTimer;

        public event EventHandler<bool>? ConnectionStatusChanged;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    ConnectionStatusChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe RestApiClient
        /// </summary>
        public RestApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(2);
            _baseUrl = "http://172.29.21.19/api"; // URL de base du serveur API

            _connectionCheckTimer = new System.Timers.Timer(5000); // Vérifie la connexion toutes les 5 secondes
            _connectionCheckTimer.Elapsed += async (s, e) => await CheckConnectionAsync();
            _connectionCheckTimer.Start();
        }

        private async Task CheckConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/health");
                IsConnected = response.IsSuccessStatusCode;
            }
            catch
            {
                IsConnected = false;
            }
        }

        /// <summary>
        /// Effectue une requête GET et retourne les données désérialisées
        /// </summary>
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Effectue une requête POST et retourne les données désérialisées
        /// </summary>
        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = JsonContent.Create(data);
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Effectue une requête PUT et retourne les données désérialisées
        /// </summary>
        public async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = JsonContent.Create(data);
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Effectue une requête DELETE et retourne un statut de succès
        /// </summary>
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _connectionCheckTimer?.Stop();
            _connectionCheckTimer?.Dispose();
            _httpClient?.Dispose();
        }
    }
} 