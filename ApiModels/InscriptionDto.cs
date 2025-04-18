using System;
using System.Collections.Generic;

namespace RamApi.ApiModels
{
    public class InscriptionDto
    {
        public int Id { get; set; }
        public int AtelierId { get; set; }
        public string NomParticipant { get; set; }
        public string PrenomParticipant { get; set; }
        public DateTime DateInscription { get; set; }
    }
}
