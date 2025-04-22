using mission.Models;
using mission.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des participants via l'API REST
    /// </summary>
    public class ParticipantController
    {
        private readonly RestApiClient _apiClient;

        /// <summary>
        /// Constructeur du contrôleur de participants
        /// </summary>
        public ParticipantController()
        {
            _apiClient = new RestApiClient();
        }

        /// <summary>
        /// Obtient tous les participants de façon asynchrone
        /// </summary>
        public async Task<List<Participant>> GetAllParticipantsAsync()
        {
            return await _apiClient.GetAsync<List<Participant>>("participants");
        }

        /// <summary>
        /// Obtient tous les participants (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Participant> GetAllParticipants()
        {
            return GetAllParticipantsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient un participant par son ID de façon asynchrone
        /// </summary>
        public async Task<Participant?> GetParticipantByIdAsync(int id)
        {
            return await _apiClient.GetAsync<Participant>($"participants/{id}");
        }

        /// <summary>
        /// Obtient un participant par son ID (méthode synchrone pour la compatibilité)
        /// </summary>
        public Participant? GetParticipantById(int id)
        {
            return GetParticipantByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Crée un nouveau participant de façon asynchrone
        /// </summary>
        public async Task CreateParticipantAsync(Participant participant)
        {
            // Validation des données
            if (string.IsNullOrWhiteSpace(participant.Nom))
                throw new ArgumentException("Le nom du participant est obligatoire.");

            if (string.IsNullOrWhiteSpace(participant.Prenom))
                throw new ArgumentException("Le prénom du participant est obligatoire.");

            await _apiClient.PostAsync<Participant>("participants", participant);
        }

        /// <summary>
        /// Crée un nouveau participant (méthode synchrone pour la compatibilité)
        /// </summary>
        public void CreateParticipant(Participant participant)
        {
            CreateParticipantAsync(participant).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Ajoute un nouveau participant (alias pour CreateParticipant)
        /// </summary>
        public void AddParticipant(Participant participant)
        {
            CreateParticipant(participant);
        }

        /// <summary>
        /// Met à jour un participant existant de façon asynchrone
        /// </summary>
        public async Task UpdateParticipantAsync(Participant participant)
        {
            // Validation des données
            if (participant.Id <= 0)
                throw new ArgumentException("L'ID du participant est invalide.");
                
            if (string.IsNullOrWhiteSpace(participant.Nom))
                throw new ArgumentException("Le nom du participant est obligatoire.");

            if (string.IsNullOrWhiteSpace(participant.Prenom))
                throw new ArgumentException("Le prénom du participant est obligatoire.");

            await _apiClient.PutAsync<Participant>($"participants/{participant.Id}", participant);
        }

        /// <summary>
        /// Met à jour un participant existant (méthode synchrone pour la compatibilité)
        /// </summary>
        public void UpdateParticipant(Participant participant)
        {
            UpdateParticipantAsync(participant).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Supprime un participant de façon asynchrone
        /// </summary>
        public async Task DeleteParticipantAsync(int id)
        {
            await _apiClient.DeleteAsync($"participants/{id}");
        }

        /// <summary>
        /// Supprime un participant (méthode synchrone pour la compatibilité)
        /// </summary>
        public void DeleteParticipant(int id)
        {
            DeleteParticipantAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les participants par type (parent ou assistante maternelle)
        /// </summary>
        public async Task<List<Participant>> GetParticipantsByTypeAsync(TypeParticipant type)
        {
            var participants = await GetAllParticipantsAsync();
            return participants
                .Where(p => p.Type == type)
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prenom)
                .ToList();
        }

        /// <summary>
        /// Obtient les participants par type (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Participant> GetParticipantsByType(TypeParticipant type)
        {
            return GetParticipantsByTypeAsync(type).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Recherche des participants par nom ou prénom de façon asynchrone
        /// </summary>
        public async Task<List<Participant>> SearchParticipantsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllParticipantsAsync();

            var participants = await GetAllParticipantsAsync();
            return participants
                .Where(p => p.Nom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            p.Prenom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prenom)
                .ToList();
        }

        /// <summary>
        /// Recherche des participants par nom ou prénom (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Participant> SearchParticipants(string searchTerm)
        {
            return SearchParticipantsAsync(searchTerm).GetAwaiter().GetResult();
        }
    }
}
