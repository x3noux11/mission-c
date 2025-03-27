using mission.Data;
using mission.Models;
using System;
using System.Collections.Generic;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des inscriptions aux ateliers
    /// </summary>
    public class InscriptionController
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructeur du contrôleur d'inscriptions
        /// </summary>
        public InscriptionController()
        {
            _context = DataContext.Instance;
        }

        /// <summary>
        /// Obtient toutes les inscriptions
        /// </summary>
        public List<Inscription> GetAllInscriptions()
        {
            return _context.GetAllInscriptions();
        }

        /// <summary>
        /// Obtient une inscription par son ID
        /// </summary>
        public Inscription? GetInscriptionById(int id)
        {
            return _context.GetInscriptionById(id);
        }

        /// <summary>
        /// Obtient les inscriptions pour un atelier spécifique
        /// </summary>
        public List<Inscription> GetInscriptionsByAtelierId(int atelierId)
        {
            return _context.GetInscriptionsByAtelierId(atelierId);
        }

        /// <summary>
        /// Obtient les inscriptions pour un participant spécifique
        /// </summary>
        public List<Inscription> GetInscriptionsByParticipantId(int participantId)
        {
            return _context.GetInscriptionsByParticipantId(participantId);
        }

        /// <summary>
        /// Crée une nouvelle inscription
        /// </summary>
        public void CreateInscription(Inscription inscription)
        {
            // Vérifier que l'atelier existe
            var atelier = _context.GetAtelierById(inscription.AtelierId);
            if (atelier == null)
                throw new ArgumentException("L'atelier spécifié n'existe pas.");

            // Vérifier que le participant existe
            var participant = _context.GetParticipantById(inscription.ParticipantId);
            if (participant == null)
                throw new ArgumentException("Le participant spécifié n'existe pas.");

            // Vérifier que le participant correspond au public cible de l'atelier
            if (atelier.PublicConcerne == TypePublic.Parents && participant.Type != TypeParticipant.Parent)
                throw new InvalidOperationException("Cet atelier est réservé aux parents.");

            if (atelier.PublicConcerne == TypePublic.AssistantesMaternelles && participant.Type != TypeParticipant.AssistanteMaternelle)
                throw new InvalidOperationException("Cet atelier est réservé aux assistantes maternelles.");

            // Vérifier que l'atelier n'est pas complet
            if (atelier.EstComplet)
                throw new InvalidOperationException("Cet atelier est complet.");

            // Vérifier que l'atelier n'est pas déjà passé
            if (atelier.DateDebut < DateTime.Now)
                throw new InvalidOperationException("Impossible de s'inscrire à un atelier passé.");

            // Vérifier que le participant n'est pas déjà inscrit à cet atelier
            var existingInscription = _context.GetAllInscriptions()
                .FirstOrDefault(i => i.AtelierId == inscription.AtelierId && i.ParticipantId == inscription.ParticipantId);

            if (existingInscription != null)
                throw new InvalidOperationException("Ce participant est déjà inscrit à cet atelier.");

            // Ajouter l'inscription
            _context.AddInscription(inscription);
        }

        /// <summary>
        /// Met à jour une inscription existante
        /// </summary>
        public void UpdateInscription(Inscription inscription)
        {
            var existingInscription = _context.GetInscriptionById(inscription.Id);
            if (existingInscription == null)
                throw new ArgumentException("L'inscription spécifiée n'existe pas.");

            _context.UpdateInscription(inscription);
        }

        /// <summary>
        /// Supprime une inscription
        /// </summary>
        public void DeleteInscription(int id)
        {
            var inscription = _context.GetInscriptionById(id);
            if (inscription == null)
                throw new ArgumentException("L'inscription spécifiée n'existe pas.");

            _context.DeleteInscription(id);
        }

        /// <summary>
        /// Marque un participant comme présent à un atelier
        /// </summary>
        public void MarquerPresence(int inscriptionId, bool present)
        {
            var inscription = _context.GetInscriptionById(inscriptionId);
            if (inscription == null)
                throw new ArgumentException("L'inscription spécifiée n'existe pas.");

            inscription.Presence = present;
            _context.UpdateInscription(inscription);
        }

        /// <summary>
        /// Obtient la liste des présences pour un atelier
        /// </summary>
        public List<Inscription> GetPresencesByAtelierId(int atelierId)
        {
            return _context.GetInscriptionsByAtelierId(atelierId);
        }

        /// <summary>
        /// Génère une liste d'émargement pour un atelier
        /// </summary>
        public List<string> GenererListeEmargement(int atelierId)
        {
            var atelier = _context.GetAtelierById(atelierId);
            if (atelier == null)
                throw new ArgumentException("L'atelier spécifié n'existe pas.");

            var inscriptions = _context.GetInscriptionsByAtelierId(atelierId);
            
            return inscriptions
                .OrderBy(i => i.Participant?.Nom)
                .ThenBy(i => i.Participant?.Prenom)
                .Select(i => $"{i.Participant?.Nom} {i.Participant?.Prenom}")
                .ToList();
        }
    }
}
