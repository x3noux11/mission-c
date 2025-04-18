using System;
using System.Collections.Generic;
using System.Linq;
using RamApi.ApiControllers;
using RamApi.ApiModels;



namespace RamApi.ApiTests
{
    public class ApiIntegrationTests
    {
        private readonly AteliersApiController _controller;

        public ApiIntegrationTests()
        {
            _controller = new AteliersApiController();
        }

        public void RunTests()
        {
            Console.WriteLine("Running API Integration Tests...");

            TestCreateAndGetAtelier();
            TestAddAndGetInscriptions();
            TestAtelierIsFull();
            TestGetNonExistentAtelier();
            TestAddInscriptionToNonExistentAtelier();

            Console.WriteLine("API Integration Tests Completed.");
        }

        private void TestCreateAndGetAtelier()
        {
            Console.WriteLine("\n--- Test: Create and Get Atelier ---");
            var newAtelierDto = new AtelierDto
            {
                Nom = "Atelier Test API",
                DateHeureDebut = DateTime.Now.AddDays(7),
                DateHeureFin = DateTime.Now.AddDays(7).AddHours(2),
                NombrePlaces = 10,
                PublicConcerne = "Assistantes Maternelles"
            };

            var createdAtelier = _controller.CreateAtelier(newAtelierDto);
            Assert(createdAtelier != null, "CreateAtelier: Atelier should be created");
            Assert(createdAtelier.Nom == newAtelierDto.Nom, "CreateAtelier: Name mismatch");
            Console.WriteLine($"Atelier created with ID: {createdAtelier.Id}");

            var fetchedAtelier = _controller.GetAtelierById(createdAtelier.Id);
            Assert(fetchedAtelier != null, "GetAtelierById: Atelier should be found");
            Assert(fetchedAtelier.Id == createdAtelier.Id, "GetAtelierById: ID mismatch");
            Console.WriteLine($"Successfully fetched atelier with ID: {fetchedAtelier.Id}");

            var allAteliers = _controller.GetAteliers();
            Assert(allAteliers.Any(a => a.Id == createdAtelier.Id), "GetAteliers: Created atelier should be in the list");
            Console.WriteLine("GetAteliers returned the created atelier.");
        }

        private void TestAddAndGetInscriptions()
        {
            Console.WriteLine("\n--- Test: Add and Get Inscriptions ---");
            
            var atelierDto = new AtelierDto
            {
                Nom = "Atelier Inscription Test",
                DateHeureDebut = DateTime.Now.AddDays(10),
                DateHeureFin = DateTime.Now.AddDays(10).AddHours(1),
                NombrePlaces = 2,
                PublicConcerne = "Parents"
            };
            var atelier = _controller.CreateAtelier(atelierDto);
            Console.WriteLine($"Created atelier for inscription test with ID: {atelier.Id}");

            var inscription1Dto = new InscriptionDto
            {
                NomParticipant = "Dupont",
                PrenomParticipant = "Jean"
            };
            var createdInscription1 = _controller.AddInscription(atelier.Id, inscription1Dto);
            Assert(createdInscription1 != null, "AddInscription 1: Inscription should be created");
            Assert(createdInscription1.NomParticipant == inscription1Dto.NomParticipant, "AddInscription 1: Name mismatch");
            Console.WriteLine($"Inscription 1 added with ID: {createdInscription1.Id}");

             var inscription2Dto = new InscriptionDto
            {
                NomParticipant = "Martin",
                PrenomParticipant = "Alice"
            };
             var createdInscription2 = _controller.AddInscription(atelier.Id, inscription2Dto);
            Assert(createdInscription2 != null, "AddInscription 2: Inscription should be created");
            Console.WriteLine($"Inscription 2 added with ID: {createdInscription2.Id}");

            var inscriptions = _controller.GetInscriptionsForAtelier(atelier.Id);
            Assert(inscriptions != null, "GetInscriptionsForAtelier: Should return a list");
            Assert(inscriptions.Count == 2, $"GetInscriptionsForAtelier: Should have 2 inscriptions, but found {inscriptions.Count}");
            Assert(inscriptions.Any(i => i.Id == createdInscription1.Id), "GetInscriptionsForAtelier: Inscription 1 missing");
            Assert(inscriptions.Any(i => i.Id == createdInscription2.Id), "GetInscriptionsForAtelier: Inscription 2 missing");
            Console.WriteLine($"Successfully fetched {inscriptions.Count} inscriptions for atelier ID: {atelier.Id}");
        }

        private void TestAtelierIsFull()
        {
            Console.WriteLine("\n--- Test: Atelier Is Full ---");
            var atelierDto = new AtelierDto
            {
                Nom = "Atelier Full Test",
                DateHeureDebut = DateTime.Now.AddDays(5),
                DateHeureFin = DateTime.Now.AddDays(5).AddHours(1),
                NombrePlaces = 1, 
                PublicConcerne = "Parents"
            };
            var atelier = _controller.CreateAtelier(atelierDto);
            Console.WriteLine($"Created atelier for full test with ID: {atelier.Id}");

            var inscription1Dto = new InscriptionDto { NomParticipant = "Test", PrenomParticipant = "Full1" };
            _controller.AddInscription(atelier.Id, inscription1Dto);
            Console.WriteLine("Added first inscription.");

            var inscription2Dto = new InscriptionDto { NomParticipant = "Test", PrenomParticipant = "Full2" };
            try
            {
                _controller.AddInscription(atelier.Id, inscription2Dto);
                Assert(false, "AddInscription (Full): Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert(ex.Message == "Atelier is full.", "AddInscription (Full): Incorrect exception message");
                Console.WriteLine("Correctly threw exception when adding inscription to full atelier.");
            }
        }

         private void TestGetNonExistentAtelier()
        {
            Console.WriteLine("\n--- Test: Get Non-Existent Atelier ---");
            var nonExistentId = 9999;
            var atelier = _controller.GetAtelierById(nonExistentId);
            Assert(atelier == null, "GetAtelierById (Non-Existent): Should return null");
            Console.WriteLine($"Correctly returned null for non-existent atelier ID: {nonExistentId}");

             try
            {
                _controller.GetInscriptionsForAtelier(nonExistentId);
                Assert(false, "GetInscriptionsForAtelier (Non-Existent): Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert(ex.Message == $"Atelier with ID {nonExistentId} not found.", "GetInscriptionsForAtelier (Non-Existent): Incorrect exception message");
                Console.WriteLine("Correctly threw exception when getting inscriptions for non-existent atelier.");
            }
        }

        private void TestAddInscriptionToNonExistentAtelier()
        {
             Console.WriteLine("\n--- Test: Add Inscription to Non-Existent Atelier ---");
             var nonExistentAtelierId = 8888;
             var inscriptionDto = new InscriptionDto { NomParticipant = "Test", PrenomParticipant = "NonExistent" };

             try
            {
                _controller.AddInscription(nonExistentAtelierId, inscriptionDto);
                Assert(false, "AddInscription (Non-Existent Atelier): Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert(ex.Message == $"Atelier with ID {nonExistentAtelierId} not found.", "AddInscription (Non-Existent Atelier): Incorrect exception message");
                Console.WriteLine("Correctly threw exception when adding inscription to non-existent atelier.");
            }
        }


        
        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Assertion Failed: {message}");
                Console.ResetColor();
                
                
            }
            else
            {
                 Console.ForegroundColor = ConsoleColor.Green;
                 Console.WriteLine($"Assertion Passed: {message}");
                 Console.ResetColor();
            }
        }
    }
}
