using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace mission.Models
{
    /// <summary>
    /// Représente un participant aux ateliers du RAM
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Identifiant unique du participant
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du participant
        /// </summary>
        [DisplayName("Nom")]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Prénom du participant
        /// </summary>
        [DisplayName("Prénom")]
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse email du participant
        /// </summary>
        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Numéro de téléphone du participant
        /// </summary>
        [DisplayName("Téléphone")]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// Type du participant (parent ou assistante maternelle)
        /// </summary>
        [DisplayName("Type")]
        public TypeParticipant Type { get; set; }

        /// <summary>
        /// Liste des inscriptions du participant
        /// </summary>
        public List<Inscription> Inscriptions { get; set; } = new List<Inscription>();

        /// <summary>
        /// Retourne le nom complet du participant
        /// </summary>
        [DisplayName("Nom complet")]
        public string NomComplet => $"{Prenom} {Nom}";
    }

    /// <summary>
    /// Définit les types de participants aux ateliers
    /// </summary>
    public enum TypeParticipant
    {
        /// <summary>
        /// Parent d'un enfant
        /// </summary>
        [Description("Parent")]
        Parent,

        /// <summary>
        /// Assistante maternelle
        /// </summary>
        [Description("Assistante maternelle")]
        AssistanteMaternelle
    }
}
