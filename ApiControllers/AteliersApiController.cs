using System;
using System.Collections.Generic;
using System.Linq;
// Assuming your models are in the 'mission.Models' namespace based on project name 'mission'
// Adjust if necessary
using mission.Models; 
using RamApi.ApiModels; // Using the DTOs we created

namespace RamApi.ApiControllers
{
    // This class simulates an API controller. 
    // In a real ASP.NET Core project, it would inherit from ControllerBase
    // and use attributes like [ApiController], [Route], [HttpGet], [HttpPost], etc.
    public class AteliersApiController
    {
        // --- Data Access Simulation ---
        // In a real app, this would likely involve a database context 
        // or a service injected via dependency injection.
        // For now, let's simulate with in-memory lists based on your Models.
        // IMPORTANT: This is temporary data for demonstration. 
        // You'll need to integrate this with your actual data persistence (from the 'Data' folder).
        private static List<Atelier> _ateliers = new List<Atelier>();
        private static List<Inscription> _inscriptions = new List<Inscription>();
        private static int _nextAtelierId = 1;
        private static int _nextInscriptionId = 1;

        // --- Constructor (Example for potential dependency injection) ---
        // public AteliersApiController(IDataService dataService) { ... }

        // --- API Endpoint Simulations ---

        // GET /api/ateliers
        public List<AtelierDto> GetAteliers()
        {
            // Map domain models to DTOs
            return _ateliers.Select(a => MapToDto(a)).ToList();
        }

        // GET /api/ateliers/{id}
        public AtelierDto GetAtelierById(int id)
        {
            var atelier = _ateliers.FirstOrDefault(a => a.Id == id);
            return atelier == null ? null : MapToDto(atelier);
        }

        // POST /api/ateliers
        public AtelierDto CreateAtelier(AtelierDto atelierDto)
        {
            if (atelierDto == null)
            {
                // In a real API, you'd return BadRequest
                throw new ArgumentNullException(nameof(atelierDto));
            }

            var newAtelier = new Atelier
            {
                Id = _nextAtelierId++, // Simulate ID generation
                Nom = atelierDto.Nom,
                DateHeureDebut = atelierDto.DateHeureDebut,
                DateHeureFin = atelierDto.DateHeureFin,
                NombrePlaces = atelierDto.NombrePlaces,
                PublicConcerne = atelierDto.PublicConcerne,
                Inscriptions = new List<Inscription>() // Initialize inscriptions list
            };

            _ateliers.Add(newAtelier);
            
            // Return the created atelier DTO (usually with the generated ID)
            return MapToDto(newAtelier); 
        }

        // POST /api/ateliers/{atelierId}/inscriptions
        public InscriptionDto AddInscription(int atelierId, InscriptionDto inscriptionDto)
        {
             if (inscriptionDto == null)
            {
                throw new ArgumentNullException(nameof(inscriptionDto));
            }

            var atelier = _ateliers.FirstOrDefault(a => a.Id == atelierId);
            if (atelier == null)
            {
                // In a real API, return NotFound
                throw new Exception($"Atelier with ID {atelierId} not found.");
            }

            // Basic check for available places (more robust logic might be needed)
            if (atelier.Inscriptions.Count >= atelier.NombrePlaces)
            {
                 // In a real API, return Conflict or BadRequest
                throw new Exception("Atelier is full.");
            }

            var newInscription = new Inscription
            {
                Id = _nextInscriptionId++, // Simulate ID generation
                AtelierId = atelierId,
                // Assuming Inscription model has these fields, adjust if needed based on Participant.cs
                NomParticipant = inscriptionDto.NomParticipant, 
                PrenomParticipant = inscriptionDto.PrenomParticipant,
                DateInscription = DateTime.UtcNow // Or use inscriptionDto.DateInscription if provided
            };

            _inscriptions.Add(newInscription); // Add to global list
            atelier.Inscriptions.Add(newInscription); // Add reference to Atelier's list

            // Map the created inscription to its DTO
            return MapToDto(newInscription);
        }

        // GET /api/ateliers/{atelierId}/inscriptions
        public List<InscriptionDto> GetInscriptionsForAtelier(int atelierId)
        {
             var atelier = _ateliers.FirstOrDefault(a => a.Id == atelierId);
            if (atelier == null)
            {
                // In a real API, return NotFound
                 throw new Exception($"Atelier with ID {atelierId} not found.");
            }

            // Find inscriptions associated with this atelier ID
            // This assumes Inscription has an AtelierId property.
            var inscriptionsForAtelier = _inscriptions.Where(i => i.AtelierId == atelierId).ToList();

            // Map to DTOs
            return inscriptionsForAtelier.Select(i => MapToDto(i)).ToList();
        }


        // --- Mapping Helpers ---
        // In a real project, consider using libraries like AutoMapper

        private AtelierDto MapToDto(Atelier atelier)
        {
            if (atelier == null) return null;
            return new AtelierDto
            {
                Id = atelier.Id,
                Nom = atelier.Nom,
                DateHeureDebut = atelier.DateHeureDebut,
                DateHeureFin = atelier.DateHeureFin,
                NombrePlaces = atelier.NombrePlaces,
                PublicConcerne = atelier.PublicConcerne
            };
        }

        private InscriptionDto MapToDto(Inscription inscription)
        {
             if (inscription == null) return null;
            return new InscriptionDto
            {
                Id = inscription.Id,
                AtelierId = inscription.AtelierId,
                NomParticipant = inscription.NomParticipant,
                PrenomParticipant = inscription.PrenomParticipant,
                DateInscription = inscription.DateInscription
            };
        }
        
        // --- TODO ---
        // - Implement PUT/PATCH for updates
        // - Implement DELETE for ateliers and inscriptions
        // - Integrate with actual data persistence layer (replace in-memory lists)
        // - Add proper error handling and validation
        // - Add authentication/authorization if needed
    }
}
