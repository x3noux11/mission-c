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
    public class RestApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initialise une nouvelle instance de la classe RestApiClient
        /// </summary>
        public RestApiClient()
        {
            _httpClient = new HttpClient();
            _baseUrl = "http://172.29.21.19/api"; // URL de base du serveur API
        }

        /// <summary>
        /// Effectue une requête GET et retourne les données désérialisées
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions) 
                    ?? throw new Exception($"Échec de désérialisation de la réponse de {endpoint}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la requête GET à {endpoint}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Effectue une requête POST et retourne les données désérialisées
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = JsonContent.Create(data);
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions) 
                    ?? throw new Exception($"Échec de désérialisation de la réponse de {endpoint}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la requête POST à {endpoint}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Effectue une requête PUT et retourne les données désérialisées
        /// </summary>
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = JsonContent.Create(data);
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions) 
                    ?? throw new Exception($"Échec de désérialisation de la réponse de {endpoint}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la requête PUT à {endpoint}: {ex.Message}", ex);
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
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la requête DELETE à {endpoint}: {ex.Message}", ex);
            }
        }
    }
} 