using mission.Data;
using mission.Models;
using System;
using System.Collections.Generic;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des participants
    /// </summary>
    public class ParticipantController
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructeur du contrôleur de participants
        /// </summary>
        public ParticipantController()
        {
            _context = DataContext.Instance;
        }

        /// <summary>
        /// Obtient tous les participants
        /// </summary>
        public List<Participant> GetAllParticipants()
        {
            return _context.GetAllParticipants();
        }

        /// <summary>
        /// Obtient un participant par son ID
        /// </summary>
        public Participant? GetParticipantById(int id)
        {
            return _context.GetParticipantById(id);
        }

        /// <summary>
        /// Crée un nouveau participant
        /// </summary>
        public void CreateParticipant(Participant participant)
        {
            // Validation des données
            if (string.IsNullOrWhiteSpace(participant.Nom))
                throw new ArgumentException("Le nom du participant est obligatoire.");

            if (string.IsNullOrWhiteSpace(participant.Prenom))
                throw new ArgumentException("Le prénom du participant est obligatoire.");

            _context.AddParticipant(participant);
        }

        /// <summary>
        /// Ajoute un nouveau participant
        /// </summary>
        public void AddParticipant(Participant participant)
        {
            // Réutiliser la méthode existante pour la validation
            CreateParticipant(participant);
        }

        /// <summary>
        /// Met à jour un participant existant
        /// </summary>
        public void UpdateParticipant(Participant participant)
        {
            // Validation des données
            if (participant.Id <= 0)
                throw new ArgumentException("L'ID du participant est invalide.");
                
            if (string.IsNullOrWhiteSpace(participant.Nom))
                throw new ArgumentException("Le nom du participant est obligatoire.");

            if (string.IsNullOrWhiteSpace(participant.Prenom))
                throw new ArgumentException("Le prénom du participant est obligatoire.");

            _context.UpdateParticipant(participant);
        }

        /// <summary>
        /// Supprime un participant
        /// </summary>
        public void DeleteParticipant(int id)
        {
            var participant = _context.GetParticipantById(id);
            if (participant == null)
                throw new ArgumentException("Le participant spécifié n'existe pas.");

            // Vérifier si le participant a des inscriptions
            if (participant.Inscriptions.Count > 0)
                throw new InvalidOperationException("Impossible de supprimer un participant qui a des inscriptions.");

            _context.DeleteParticipant(id);
        }

        /// <summary>
        /// Obtient les participants par type (parent ou assistante maternelle)
        /// </summary>
        public List<Participant> GetParticipantsByType(TypeParticipant type)
        {
            return _context.GetAllParticipants()
                .Where(p => p.Type == type)
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prenom)
                .ToList();
        }

        /// <summary>
        /// Recherche des participants par nom ou prénom
        /// </summary>
        public List<Participant> SearchParticipants(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllParticipants();

            return _context.GetAllParticipants()
                .Where(p => p.Nom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            p.Prenom.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Nom)
                .ThenBy(p => p.Prenom)
                .ToList();
        }
    }
}
