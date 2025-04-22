using mission.Models;
using mission.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des inscriptions via l'API REST
    /// </summary>
    public class InscriptionController
    {
        private readonly RestApiClient _apiClient;
        private readonly AtelierController _atelierController;
        private readonly ParticipantController _participantController;

        /// <summary>
        /// Constructeur du contrôleur d'inscriptions
        /// </summary>
        public InscriptionController()
        {
            _apiClient = new RestApiClient();
            _atelierController = new AtelierController();
            _participantController = new ParticipantController();
        }

        /// <summary>
        /// Obtient toutes les inscriptions de façon asynchrone
        /// </summary>
        public async Task<List<Inscription>> GetAllInscriptionsAsync()
        {
            return await _apiClient.GetAsync<List<Inscription>>("inscriptions");
        }

        /// <summary>
        /// Obtient toutes les inscriptions (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Inscription> GetAllInscriptions()
        {
            return GetAllInscriptionsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient une inscription par son ID de façon asynchrone
        /// </summary>
        public async Task<Inscription?> GetInscriptionByIdAsync(int id)
        {
            return await _apiClient.GetAsync<Inscription>($"inscriptions/{id}");
        }

        /// <summary>
        /// Obtient une inscription par son ID (méthode synchrone pour la compatibilité)
        /// </summary>
        public Inscription? GetInscriptionById(int id)
        {
            return GetInscriptionByIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les inscriptions pour un atelier spécifique de façon asynchrone
        /// </summary>
        public async Task<List<Inscription>> GetInscriptionsByAtelierIdAsync(int atelierId)
        {
            var inscriptions = await GetAllInscriptionsAsync();
            return inscriptions.Where(i => i.AtelierId == atelierId).ToList();
        }

        /// <summary>
        /// Obtient les inscriptions pour un atelier spécifique (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Inscription> GetInscriptionsByAtelierId(int atelierId)
        {
            return GetInscriptionsByAtelierIdAsync(atelierId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtient les inscriptions pour un participant spécifique de façon asynchrone
        /// </summary>
        public async Task<List<Inscription>> GetInscriptionsByParticipantIdAsync(int participantId)
        {
            var inscriptions = await GetAllInscriptionsAsync();
            return inscriptions.Where(i => i.ParticipantId == participantId).ToList();
        }

        /// <summary>
        /// Obtient les inscriptions pour un participant spécifique (méthode synchrone pour la compatibilité)
        /// </summary>
        public List<Inscription> GetInscriptionsByParticipantId(int participantId)
        {
            return GetInscriptionsByParticipantIdAsync(participantId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Crée une nouvelle inscription de façon asynchrone
        /// </summary>
        public async Task CreateInscriptionAsync(Inscription inscription)
        {
            // Validation des données
            if (inscription.AtelierId <= 0)
                throw new ArgumentException("L'ID de l'atelier est invalide.");

            if (inscription.ParticipantId <= 0)
                throw new ArgumentException("L'ID du participant est invalide.");

            // Vérifier que l'atelier existe
            var atelier = await _atelierController.GetAtelierByIdAsync(inscription.AtelierId);
            if (atelier == null)
                throw new ArgumentException("L'atelier spécifié n'existe pas.");

            // Vérifier que le participant existe
            var participant = await _participantController.GetParticipantByIdAsync(inscription.ParticipantId);
            if (participant == null)
                throw new ArgumentException("Le participant spécifié n'existe pas.");

            // Vérifier que le participant n'est pas déjà inscrit
            var existingInscriptions = await GetInscriptionsByAtelierIdAsync(inscription.AtelierId);
            if (existingInscriptions.Any(i => i.ParticipantId == inscription.ParticipantId))
                throw new InvalidOperationException("Ce participant est déjà inscrit à cet atelier.");

            // Vérifier qu'il reste des places disponibles
            int placesOccupees = existingInscriptions.Count;
            if (placesOccupees >= atelier.PlacesDisponibles)
                throw new InvalidOperationException("Plus de places disponibles pour cet atelier.");

            // Définir la date d'inscription
            inscription.DateInscription = DateTime.Now;

            await _apiClient.PostAsync<Inscription>("inscriptions", inscription);
        }

        /// <summary>
        /// Crée une nouvelle inscription (méthode synchrone pour la compatibilité)
        /// </summary>
        public void CreateInscription(Inscription inscription)
        {
            CreateInscriptionAsync(inscription).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Met à jour une inscription existante de façon asynchrone
        /// </summary>
        public async Task UpdateInscriptionAsync(Inscription inscription)
        {
            // Validation des données
            if (inscription.Id <= 0)
                throw new ArgumentException("L'ID de l'inscription est invalide.");

            await _apiClient.PutAsync<Inscription>($"inscriptions/{inscription.Id}", inscription);
        }

        /// <summary>
        /// Met à jour une inscription existante (méthode synchrone pour la compatibilité)
        /// </summary>
        public void UpdateInscription(Inscription inscription)
        {
            UpdateInscriptionAsync(inscription).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Met à jour le statut de présence d'une inscription de façon asynchrone
        /// </summary>
        public async Task UpdatePresenceAsync(int inscriptionId, bool present)
        {
            var inscription = await GetInscriptionByIdAsync(inscriptionId);
            if (inscription == null)
                throw new ArgumentException("L'inscription spécifiée n'existe pas.");

            inscription.Presence = present;
            await UpdateInscriptionAsync(inscription);
        }

        /// <summary>
        /// Met à jour le statut de présence d'une inscription (méthode synchrone pour la compatibilité)
        /// </summary>
        public void UpdatePresence(int inscriptionId, bool present)
        {
            UpdatePresenceAsync(inscriptionId, present).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Supprime une inscription de façon asynchrone
        /// </summary>
        public async Task DeleteInscriptionAsync(int id)
        {
            await _apiClient.DeleteAsync($"inscriptions/{id}");
        }

        /// <summary>
        /// Supprime une inscription (méthode synchrone pour la compatibilité)
        /// </summary>
        public void DeleteInscription(int id)
        {
            DeleteInscriptionAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Génère une liste d'émargement pour un atelier
        /// </summary>
        public List<string> GenererListeEmargement(int atelierId)
        {
            var atelier = _atelierController.GetAtelierById(atelierId);
            if (atelier == null)
                throw new ArgumentException("L'atelier spécifié n'existe pas.");

            var inscriptions = GetInscriptionsByAtelierId(atelierId);
            
            return inscriptions
                .OrderBy(i => i.Participant?.Nom)
                .ThenBy(i => i.Participant?.Prenom)
                .Select(i => $"{i.Participant?.Nom} {i.Participant?.Prenom}")
                .ToList();
        }
    }
}
