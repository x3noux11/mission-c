using System;
using System.Collections.Generic;

namespace RamApi.ApiModels
{
    /// <summary>
    /// Data Transfer Object for Atelier information.
    /// </summary>
    public class AtelierDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime DateHeureDebut { get; set; }
        public DateTime DateHeureFin { get; set; }
        public int NombrePlaces { get; set; }
        public string PublicConcerne { get; set; } // e.g., "Parents", "Assistantes Maternelles"
        // On pourrait ajouter d'autres propriétés si nécessaire, comme une description.
    }
}
