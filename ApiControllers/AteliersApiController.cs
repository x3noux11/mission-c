using System;
using System.Collections.Generic;
using System.Linq;
using mission.Models;
using RamApi.ApiModels;

namespace RamApi.ApiControllers
{
    public class AteliersApiController
    {
        private static List<Atelier> _ateliers = new List<Atelier>();
        private static List<Inscription> _inscriptions = new List<Inscription>();
        private static int _nextAtelierId = 1;
        private static int _nextInscriptionId = 1;

        public List<AtelierDto> GetAteliers()
        {
            return _ateliers.Select(a => MapToDto(a)).ToList();
        }

        public AtelierDto GetAtelierById(int id)
        {
            var atelier = _ateliers.FirstOrDefault(a => a.Id == id);
            return atelier == null ? null : MapToDto(atelier);
        }

        public AtelierDto CreateAtelier(AtelierDto atelierDto)
        {
            if (atelierDto == null)
            {
                throw new ArgumentNullException(nameof(atelierDto));
            }

            // Convertir le type de public
            TypePublic publicConcerne;
            if (atelierDto.PublicConcerne == "Parents")
                publicConcerne = TypePublic.Parents;
            else if (atelierDto.PublicConcerne == "Assistantes Maternelles")
                publicConcerne = TypePublic.AssistantesMaternelles;
            else
                publicConcerne = TypePublic.Tous;

            // Calculer la durée en minutes
            int dureeMinutes = (int)(atelierDto.DateHeureFin - atelierDto.DateHeureDebut).TotalMinutes;

            var newAtelier = new Atelier
            {
                Id = _nextAtelierId++,
                Titre = atelierDto.Nom,
                Description = "", // Valeur par défaut
                DateDebut = atelierDto.DateHeureDebut,
                Duree = dureeMinutes,
                NombrePlaces = atelierDto.NombrePlaces,
                PublicConcerne = publicConcerne,
                Inscriptions = new List<Inscription>()
            };

            _ateliers.Add(newAtelier);
            
            return MapToDto(newAtelier);
        }

        public InscriptionDto AddInscription(int atelierId, InscriptionDto inscriptionDto)
        {
            if (inscriptionDto == null)
            {
                throw new ArgumentNullException(nameof(inscriptionDto));
            }

            var atelier = _ateliers.FirstOrDefault(a => a.Id == atelierId);
            if (atelier == null)
            {
                throw new Exception($"Atelier with ID {atelierId} not found.");
            }

            if (atelier.EstComplet)
            {
                throw new Exception("Atelier is full.");
            }

            // Créer un participant fictif pour les tests API
            var participant = new Participant
            {
                Id = 999, // ID fictif
                Nom = inscriptionDto.NomParticipant,
                Prenom = inscriptionDto.PrenomParticipant,
                Type = TypeParticipant.Parent
            };

            var newInscription = new Inscription
            {
                Id = _nextInscriptionId++,
                AtelierId = atelierId,
                ParticipantId = participant.Id,
                Participant = participant,
                DateInscription = DateTime.Now,
                Commentaires = "",
                Presence = false
            };

            _inscriptions.Add(newInscription);
            atelier.Inscriptions.Add(newInscription);

            return MapToDto(newInscription);
        }

        public List<InscriptionDto> GetInscriptionsForAtelier(int atelierId)
        {
            var atelier = _ateliers.FirstOrDefault(a => a.Id == atelierId);
            if (atelier == null)
            {
                throw new Exception($"Atelier with ID {atelierId} not found.");
            }

            var inscriptionsForAtelier = _inscriptions.Where(i => i.AtelierId == atelierId).ToList();
            return inscriptionsForAtelier.Select(i => MapToDto(i)).ToList();
        }

        private AtelierDto MapToDto(Atelier atelier)
        {
            if (atelier == null) return null;
            
            string publicConcerneStr;
            switch (atelier.PublicConcerne)
            {
                case TypePublic.Parents:
                    publicConcerneStr = "Parents";
                    break;
                case TypePublic.AssistantesMaternelles:
                    publicConcerneStr = "Assistantes Maternelles";
                    break;
                default:
                    publicConcerneStr = "Tous publics";
                    break;
            }
            
            return new AtelierDto
            {
                Id = atelier.Id,
                Nom = atelier.Titre,
                DateHeureDebut = atelier.DateDebut,
                DateHeureFin = atelier.DateFin,
                NombrePlaces = atelier.NombrePlaces,
                PublicConcerne = publicConcerneStr
            };
        }

        private InscriptionDto MapToDto(Inscription inscription)
        {
            if (inscription == null) return null;
            
            return new InscriptionDto
            {
                Id = inscription.Id,
                AtelierId = inscription.AtelierId,
                NomParticipant = inscription.Participant?.Nom ?? "Inconnu",
                PrenomParticipant = inscription.Participant?.Prenom ?? "Inconnu",
                DateInscription = inscription.DateInscription
            };
        }
    }
}