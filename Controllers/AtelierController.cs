using mission.Data;
using mission.Models;
using System;
using System.Collections.Generic;

namespace mission.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des ateliers
    /// </summary>
    public class AtelierController
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructeur du contrôleur d'ateliers
        /// </summary>
        public AtelierController()
        {
            _context = DataContext.Instance;
        }

        /// <summary>
        /// Obtient tous les ateliers
        /// </summary>
        public List<Atelier> GetAllAteliers()
        {
            return _context.GetAllAteliers();
        }

        /// <summary>
        /// Obtient un atelier par son ID
        /// </summary>
        public Atelier? GetAtelierById(int id)
        {
            return _context.GetAtelierById(id);
        }

        /// <summary>
        /// Crée un nouvel atelier
        /// </summary>
        public void CreateAtelier(Atelier atelier)
        {
            // Validation des données
            if (string.IsNullOrWhiteSpace(atelier.Titre))
                throw new ArgumentException("Le titre de l'atelier est obligatoire.");

            if (atelier.DateDebut < DateTime.Today)
                throw new ArgumentException("La date de l'atelier doit être dans le futur.");

            if (atelier.Duree <= 0)
                throw new ArgumentException("La durée de l'atelier doit être positive.");

            if (atelier.NombrePlaces <= 0)
                throw new ArgumentException("Le nombre de places doit être positif.");

            _context.AddAtelier(atelier);
        }

        /// <summary>
        /// Met à jour un atelier existant
        /// </summary>
        public void UpdateAtelier(Atelier atelier)
        {
            // Validation des données
            if (string.IsNullOrWhiteSpace(atelier.Titre))
                throw new ArgumentException("Le titre de l'atelier est obligatoire.");

            if (atelier.Duree <= 0)
                throw new ArgumentException("La durée de l'atelier doit être positive.");

            if (atelier.NombrePlaces <= 0)
                throw new ArgumentException("Le nombre de places doit être positif.");

            // Vérifier si le nombre de places a été réduit
            var existingAtelier = _context.GetAtelierById(atelier.Id);
            if (existingAtelier != null && atelier.NombrePlaces < existingAtelier.Inscriptions.Count)
                throw new ArgumentException("Impossible de réduire le nombre de places en dessous du nombre d'inscrits.");

            _context.UpdateAtelier(atelier);
        }

        /// <summary>
        /// Supprime un atelier
        /// </summary>
        public void DeleteAtelier(int id)
        {
            var atelier = _context.GetAtelierById(id);
            if (atelier == null)
                throw new ArgumentException("L'atelier spécifié n'existe pas.");

            // Vérifier si l'atelier a des inscriptions
            if (atelier.Inscriptions.Count > 0)
                throw new InvalidOperationException("Impossible de supprimer un atelier qui a des inscriptions.");

            _context.DeleteAtelier(id);
        }

        /// <summary>
        /// Obtient les ateliers à venir
        /// </summary>
        public List<Atelier> GetAteliersAVenir()
        {
            return _context.GetAllAteliers()
                .Where(a => a.DateDebut > DateTime.Now)
                .OrderBy(a => a.DateDebut)
                .ToList();
        }

        /// <summary>
        /// Obtient les ateliers passés
        /// </summary>
        public List<Atelier> GetAteliersPasses()
        {
            return _context.GetAllAteliers()
                .Where(a => a.DateDebut < DateTime.Now)
                .OrderByDescending(a => a.DateDebut)
                .ToList();
        }

        /// <summary>
        /// Obtient les ateliers pour un type de public spécifique
        /// </summary>
        public List<Atelier> GetAteliersByPublic(TypePublic typePublic)
        {
            return _context.GetAllAteliers()
                .Where(a => a.PublicConcerne == typePublic || a.PublicConcerne == TypePublic.Tous)
                .OrderBy(a => a.DateDebut)
                .ToList();
        }
    }
}
