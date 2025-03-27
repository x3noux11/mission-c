using System;
using System.ComponentModel;

namespace mission.Models
{
    /// <summary>
    /// Représente une inscription à un atelier
    /// </summary>
    public class Inscription
    {
        /// <summary>
        /// Identifiant unique de l'inscription
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'atelier concerné
        /// </summary>
        public int AtelierId { get; set; }

        /// <summary>
        /// Référence à l'atelier concerné
        /// </summary>
        public Atelier? Atelier { get; set; }

        /// <summary>
        /// Identifiant du participant
        /// </summary>
        public int ParticipantId { get; set; }

        /// <summary>
        /// Référence au participant
        /// </summary>
        public Participant? Participant { get; set; }

        /// <summary>
        /// Date et heure de l'inscription
        /// </summary>
        [DisplayName("Date d'inscription")]
        public DateTime DateInscription { get; set; } = DateTime.Now;

        /// <summary>
        /// Commentaires éventuels sur l'inscription
        /// </summary>
        [DisplayName("Commentaires")]
        public string Commentaires { get; set; } = string.Empty;

        /// <summary>
        /// Indique si le participant était présent à l'atelier
        /// </summary>
        [DisplayName("Présent")]
        public bool Presence { get; set; }
    }
}
