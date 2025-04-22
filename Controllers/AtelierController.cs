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
    public class AtelierController
    {
        private readonly RestApiClient _apiClient;

        /// <summary>
        /// Constructeur du contrôleur d'ateliers
        /// </summary>
        public AtelierController()
        {
            _apiClient = new RestApiClient();
        }

        /// <summary>
        /// Obtient tous les ateliers de façon asynchrone
        /// </summary>
        public async Task<List<Atelier>> GetAllAteliersAsync()
        {
            return await _apiClient.GetAsync<List<Atelier>>("ateliers");
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
            return await _apiClient.GetAsync<Atelier>($"ateliers/{id}");
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
        public async Task CreateAtelierAsync(Atelier atelier)
        {
            // Validation des données
            if (string.IsNullOrWhiteSpace(atelier.Titre))
                throw new ArgumentException("Le titre de l'atelier est obligatoire.");

            if (atelier.DateHeure == default)
                throw new ArgumentException("La date et l'heure de l'atelier sont obligatoires.");

            if (atelier.PlacesDisponibles <= 0)
                throw new ArgumentException("Le nombre de places disponibles doit être supérieur à zéro.");

            await _apiClient.PostAsync<Atelier>("ateliers", atelier);
        }

        /// <summary>
        /// Crée un nouvel atelier (méthode synchrone pour la compatibilité)
        /// </summary>
        public void CreateAtelier(Atelier atelier)
        {
            CreateAtelierAsync(atelier).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Met à jour un atelier existant de façon asynchrone
        /// </summary>
        public async Task UpdateAtelierAsync(Atelier atelier)
        {
            // Validation des données
            if (atelier.Id <= 0)
                throw new ArgumentException("L'ID de l'atelier est invalide.");
                
            if (string.IsNullOrWhiteSpace(atelier.Titre))
                throw new ArgumentException("Le titre de l'atelier est obligatoire.");

            if (atelier.DateHeure == default)
                throw new ArgumentException("La date et l'heure de l'atelier sont obligatoires.");

            if (atelier.PlacesDisponibles <= 0)
                throw new ArgumentException("Le nombre de places disponibles doit être supérieur à zéro.");

            await _apiClient.PutAsync<Atelier>($"ateliers/{atelier.Id}", atelier);
        }

        /// <summary>
        /// Met à jour un atelier existant (méthode synchrone pour la compatibilité)
        /// </summary>
        public void UpdateAtelier(Atelier atelier)
        {
            UpdateAtelierAsync(atelier).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Supprime un atelier de façon asynchrone
        /// </summary>
        public async Task DeleteAtelierAsync(int id)
        {
            await _apiClient.DeleteAsync($"ateliers/{id}");
        }

        /// <summary>
        /// Supprime un atelier (méthode synchrone pour la compatibilité)
        /// </summary>
        public void DeleteAtelier(int id)
        {
            DeleteAtelierAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les ateliers par période de façon asynchrone
        /// </summary>
        public async Task<List<Atelier>> GetAteliersByPeriodAsync(DateTime debut, DateTime fin)
        {
            var ateliers = await GetAllAteliersAsync();
            return ateliers
                .Where(a => a.DateHeure >= debut && a.DateHeure <= fin)
                .OrderBy(a => a.DateHeure)
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
            var ateliers = await GetAllAteliersAsync();
            return ateliers
                .Where(a => a.DateHeure > DateTime.Now)
                .OrderBy(a => a.DateHeure)
                .ToList();
        }

        /// <summary>
        /// Obtient les ateliers à venir (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Atelier> GetAteliersAVenir()
        {
            return GetAteliersAVenirAsync().GetAwaiter().GetResult();
        }
    }
}
