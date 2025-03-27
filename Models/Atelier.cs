using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace mission.Models
{
    /// <summary>
    /// Représente un atelier d'éveil proposé par le RAM
    /// </summary>
    public class Atelier
    {
        /// <summary>
        /// Identifiant unique de l'atelier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre de l'atelier
        /// </summary>
        [DisplayName("Titre")]
        public string Titre { get; set; } = string.Empty;

        /// <summary>
        /// Description détaillée de l'atelier
        /// </summary>
        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date et heure de début de l'atelier
        /// </summary>
        [DisplayName("Date et heure de début")]
        public DateTime DateDebut { get; set; }

        /// <summary>
        /// Durée de l'atelier en minutes
        /// </summary>
        [DisplayName("Durée (minutes)")]
        public int Duree { get; set; }

        /// <summary>
        /// Nombre maximal de participants
        /// </summary>
        [DisplayName("Nombre de places")]
        public int NombrePlaces { get; set; }

        /// <summary>
        /// Type de public concerné par l'atelier
        /// </summary>
        [DisplayName("Public concerné")]
        public TypePublic PublicConcerne { get; set; }

        /// <summary>
        /// Liste des inscriptions à cet atelier
        /// </summary>
        public List<Inscription> Inscriptions { get; set; } = new List<Inscription>();

        /// <summary>
        /// Retourne le nombre de places disponibles
        /// </summary>
        [DisplayName("Places disponibles")]
        public int PlacesDisponibles => NombrePlaces - Inscriptions.Count;

        /// <summary>
        /// Indique si l'atelier est complet
        /// </summary>
        public bool EstComplet => PlacesDisponibles <= 0;

        /// <summary>
        /// Retourne l'heure de fin de l'atelier
        /// </summary>
        [DisplayName("Date et heure de fin")]
        public DateTime DateFin => DateDebut.AddMinutes(Duree);
    }

    /// <summary>
    /// Définit les types de public concernés par les ateliers
    /// </summary>
    public enum TypePublic
    {
        /// <summary>
        /// Atelier réservé aux parents et leurs enfants
        /// </summary>
        [Description("Parents et enfants")]
        Parents,

        /// <summary>
        /// Atelier réservé aux assistantes maternelles et les enfants dont elles ont la garde
        /// </summary>
        [Description("Assistantes maternelles")]
        AssistantesMaternelles,

        /// <summary>
        /// Atelier ouvert à tous
        /// </summary>
        [Description("Tous publics")]
        Tous
    }
}
