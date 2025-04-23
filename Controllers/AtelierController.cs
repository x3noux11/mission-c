using mission.Models;
using mission.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des ateliers via l'API REST
    /// </summary>
    public class AtelierController : IDisposable
    {
        private readonly RestApiClient _apiClient;
        private readonly LocalStorage _localStorage;
        private List<Atelier> _localCache = new List<Atelier>();
        private bool _isInitialized = false;

        /// <summary>
        /// Constructeur du contrôleur d'ateliers
        /// </summary>
        public AtelierController()
        {
            _apiClient = new RestApiClient();
            _localStorage = new LocalStorage();
            _apiClient.ConnectionStatusChanged += OnConnectionStatusChanged;
            _ = InitializeAsync(); // Initialisation asynchrone sans attendre
        }

        public bool IsApiConnected => _apiClient.IsConnected;

        private async void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            if (isConnected && _isInitialized)
            {
                try
                {
                    await SynchronizeDataAsync();
                }
                catch
                {
                    // Ignorer les erreurs de synchronisation
                }
            }
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    _localCache = await _localStorage.LoadAsync<List<Atelier>>("ateliers") ?? new List<Atelier>();
                }
                catch
                {
                    _localCache = new List<Atelier>();
                }

                _isInitialized = true;

                if (IsApiConnected)
                {
                    try
                    {
                        await SynchronizeDataAsync();
                    }
                    catch
                    {
                        // Ignorer les erreurs de synchronisation
                    }
                }
            }
        }

        private async Task SynchronizeDataAsync()
        {
            try
            {
                var remoteAteliers = await _apiClient.GetAsync<List<Atelier>>("ateliers");
                if (remoteAteliers != null)
                {
                    _localCache = remoteAteliers;
                    await _localStorage.SaveAsync("ateliers", _localCache);
                }
            }
            catch
            {
                // En cas d'erreur, on continue avec les données locales
            }
        }

        /// <summary>
        /// Obtient tous les ateliers de façon asynchrone
        /// </summary>
        public async Task<List<Atelier>> GetAllAteliersAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
            return _localCache;
        }

        /// <summary>
        /// Obtient tous les ateliers (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Atelier> GetAllAteliers()
        {
            return GetAllAteliersAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient un atelier par son ID de façon asynchrone
        /// </summary>
        public async Task<Atelier?> GetAtelierByIdAsync(int id)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
            return _localCache.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Obtient un atelier par son ID (méthode synchrone pour la compatibilité)
        /// </summary>
        public Atelier? GetAtelierById(int id)
        {
            return GetAtelierByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Crée un nouvel atelier de façon asynchrone
        /// </summary>
        public async Task<bool> CreateAtelierAsync(Atelier atelier)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            if (string.IsNullOrWhiteSpace(atelier.Titre))
                return false;

            if (atelier.DateDebut == default)
                return false;

            if (atelier.PlacesDisponibles <= 0)
                return false;

            // Générer un ID temporaire négatif pour les nouveaux ateliers hors ligne
            if (atelier.Id == 0)
            {
                atelier.Id = _localCache.Any() ? _localCache.Min(a => a.Id) - 1 : -1;
            }

            _localCache.Add(atelier);

            try
            {
                await _localStorage.SaveAsync("ateliers", _localCache);
            }
            catch
            {
                // En cas d'erreur de sauvegarde locale, on continue
            }

            if (IsApiConnected)
            {
                try
                {
                    var createdAtelier = await _apiClient.PostAsync<Atelier>("ateliers", atelier);
                    if (createdAtelier != null)
                    {
                        var index = _localCache.FindIndex(a => a.Id == atelier.Id);
                        if (index >= 0)
                        {
                            _localCache[index] = createdAtelier;
                            await _localStorage.SaveAsync("ateliers", _localCache);
                        }
                    }
                }
                catch
                {
                    // En cas d'erreur, on garde la version locale
                }
            }

            return true;
        }

        /// <summary>
        /// Crée un nouvel atelier (méthode synchrone pour la compatibilité)
        /// </summary>
        public bool CreateAtelier(Atelier atelier)
        {
            return CreateAtelierAsync(atelier).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Met à jour un atelier existant de façon asynchrone
        /// </summary>
        public async Task<bool> UpdateAtelierAsync(Atelier atelier)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            if (atelier.Id <= 0)
                return false;
                
            if (string.IsNullOrWhiteSpace(atelier.Titre))
                return false;

            if (atelier.DateDebut == default)
                return false;

            if (atelier.PlacesDisponibles <= 0)
                return false;

            var index = _localCache.FindIndex(a => a.Id == atelier.Id);
            if (index >= 0)
            {
                _localCache[index] = atelier;

                try
                {
                    await _localStorage.SaveAsync("ateliers", _localCache);
                }
                catch
                {
                    // En cas d'erreur de sauvegarde locale, on continue
                }

                if (IsApiConnected)
                {
                    try
                    {
                        await _apiClient.PutAsync<Atelier>($"ateliers/{atelier.Id}", atelier);
                    }
                    catch
                    {
                        // En cas d'erreur, on garde la version locale
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Met à jour un atelier existant (méthode synchrone pour la compatibilité)
        /// </summary>
        public bool UpdateAtelier(Atelier atelier)
        {
            return UpdateAtelierAsync(atelier).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Supprime un atelier de façon asynchrone
        /// </summary>
        public async Task<bool> DeleteAtelierAsync(int id)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            var index = _localCache.FindIndex(a => a.Id == id);
            if (index >= 0)
            {
                _localCache.RemoveAt(index);

                try
                {
                    await _localStorage.SaveAsync("ateliers", _localCache);
                }
                catch
                {
                    // En cas d'erreur de sauvegarde locale, on continue
                }

                if (IsApiConnected)
                {
                    try
                    {
                        await _apiClient.DeleteAsync($"ateliers/{id}");
                    }
                    catch
                    {
                        // En cas d'erreur, on garde la version locale
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Supprime un atelier (méthode synchrone pour la compatibilité)
        /// </summary>
        public bool DeleteAtelier(int id)
        {
            return DeleteAtelierAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les ateliers par période de façon asynchrone
        /// </summary>
        public async Task<List<Atelier>> GetAteliersByPeriodAsync(DateTime debut, DateTime fin)
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
            return _localCache
                .Where(a => a.DateDebut >= debut && a.DateDebut <= fin)
                .OrderBy(a => a.DateDebut)
                .ToList();
        }

        /// <summary>
        /// Obtient les ateliers par période (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Atelier> GetAteliersByPeriod(DateTime debut, DateTime fin)
        {
            return GetAteliersByPeriodAsync(debut, fin).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les ateliers à venir de façon asynchrone
        /// </summary>
        public async Task<List<Atelier>> GetAteliersAVenirAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
            return _localCache
                .Where(a => a.DateDebut > DateTime.Now)
                .OrderBy(a => a.DateDebut)
                .ToList();
        }

        /// <summary>
        /// Obtient les ateliers à venir (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Atelier> GetAteliersAVenir()
        {
            return GetAteliersAVenirAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _apiClient?.Dispose();
        }
    }
}
