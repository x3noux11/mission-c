using mission.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mission.Data
{
    /// <summary>
    /// Classe responsable de l'accès aux données de l'application
    /// </summary>
    public class DataContext
    {
        private static DataContext? _instance;
        private readonly string _dataDirectory;
        private readonly string _ateliersFile;
        private readonly string _participantsFile;
        private readonly string _inscriptionsFile;

        private List<Atelier> _ateliers = new();
        private List<Participant> _participants = new();
        private List<Inscription> _inscriptions = new();

        /// <summary>
        /// Obtient l'instance unique de DataContext (pattern Singleton)
        /// </summary>
        public static DataContext Instance
        {
            get
            {
                _instance ??= new DataContext();
                return _instance;
            }
        }

        /// <summary>
        /// Constructeur privé pour implémenter le pattern Singleton
        /// </summary>
        private DataContext()
        {
            // Définir le répertoire de données dans AppData
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RAM_Ateliers");

            // Créer le répertoire s'il n'existe pas
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            // Définir les chemins des fichiers de données
            _ateliersFile = Path.Combine(_dataDirectory, "ateliers.json");
            _participantsFile = Path.Combine(_dataDirectory, "participants.json");
            _inscriptionsFile = Path.Combine(_dataDirectory, "inscriptions.json");

            // Charger les données au démarrage
            LoadData();
        }

        /// <summary>
        /// Charge toutes les données depuis les fichiers JSON
        /// </summary>
        private void LoadData()
        {
            _ateliers = LoadFromFile<List<Atelier>>(_ateliersFile) ?? new List<Atelier>();
            _participants = LoadFromFile<List<Participant>>(_participantsFile) ?? new List<Participant>();
            _inscriptions = LoadFromFile<List<Inscription>>(_inscriptionsFile) ?? new List<Inscription>();

            // Établir les relations entre les objets
            foreach (var inscription in _inscriptions)
            {
                var atelier = _ateliers.FirstOrDefault(a => a.Id == inscription.AtelierId);
                var participant = _participants.FirstOrDefault(p => p.Id == inscription.ParticipantId);

                if (atelier != null)
                {
                    inscription.Atelier = atelier;
                    atelier.Inscriptions.Add(inscription);
                }

                if (participant != null)
                {
                    inscription.Participant = participant;
                    participant.Inscriptions.Add(inscription);
                }
            }
        }

        /// <summary>
        /// Sauvegarde toutes les données dans les fichiers JSON
        /// </summary>
        private void SaveData()
        {
            // Pour éviter les références circulaires lors de la sérialisation
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            // Sauvegarde des ateliers
            SaveToFile(_ateliersFile, _ateliers, options);

            // Sauvegarde des participants
            SaveToFile(_participantsFile, _participants, options);

            // Sauvegarde des inscriptions (sans les références aux objets)
            var inscriptionsSave = _inscriptions.Select(i => new Inscription
            {
                Id = i.Id,
                AtelierId = i.AtelierId,
                ParticipantId = i.ParticipantId,
                DateInscription = i.DateInscription,
                Commentaires = i.Commentaires,
                Presence = i.Presence
            }).ToList();

            SaveToFile(_inscriptionsFile, inscriptionsSave, options);
        }

        /// <summary>
        /// Charge des données depuis un fichier JSON
        /// </summary>
        private T? LoadFromFile<T>(string fileName)
        {
            if (!File.Exists(fileName))
                return default;

            try
            {
                string jsonData = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du fichier {fileName}: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Sauvegarde des données dans un fichier JSON
        /// </summary>
        private void SaveToFile<T>(string fileName, T data, JsonSerializerOptions? options = null)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(data, options);
                File.WriteAllText(fileName, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde du fichier {fileName}: {ex.Message}");
            }
        }

        #region Méthodes d'accès aux ateliers

        /// <summary>
        /// Obtient tous les ateliers
        /// </summary>
        public List<Atelier> GetAllAteliers()
        {
            return _ateliers.ToList();
        }

        /// <summary>
        /// Obtient un atelier par son ID
        /// </summary>
        public Atelier? GetAtelierById(int id)
        {
            return _ateliers.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Ajoute un nouvel atelier
        /// </summary>
        public void AddAtelier(Atelier atelier)
        {
            // Génère un nouvel ID
            atelier.Id = _ateliers.Count > 0 ? _ateliers.Max(a => a.Id) + 1 : 1;
            _ateliers.Add(atelier);
            SaveData();
        }

        /// <summary>
        /// Met à jour un atelier existant
        /// </summary>
        public void UpdateAtelier(Atelier atelier)
        {
            var existingAtelier = _ateliers.FirstOrDefault(a => a.Id == atelier.Id);
            if (existingAtelier != null)
            {
                var index = _ateliers.IndexOf(existingAtelier);
                _ateliers[index] = atelier;
                SaveData();
            }
        }

        /// <summary>
        /// Supprime un atelier
        /// </summary>
        public void DeleteAtelier(int id)
        {
            var atelier = _ateliers.FirstOrDefault(a => a.Id == id);
            if (atelier != null)
            {
                // Supprimer également les inscriptions liées à cet atelier
                var relatedInscriptions = _inscriptions.Where(i => i.AtelierId == id).ToList();
                foreach (var inscription in relatedInscriptions)
                {
                    _inscriptions.Remove(inscription);
                }

                _ateliers.Remove(atelier);
                SaveData();
            }
        }

        #endregion

        #region Méthodes d'accès aux participants

        /// <summary>
        /// Obtient tous les participants
        /// </summary>
        public List<Participant> GetAllParticipants()
        {
            return _participants.ToList();
        }

        /// <summary>
        /// Obtient un participant par son ID
        /// </summary>
        public Participant? GetParticipantById(int id)
        {
            return _participants.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Ajoute un nouveau participant
        /// </summary>
        public void AddParticipant(Participant participant)
        {
            // Génère un nouvel ID
            participant.Id = _participants.Count > 0 ? _participants.Max(p => p.Id) + 1 : 1;
            _participants.Add(participant);
            SaveData();
        }

        /// <summary>
        /// Met à jour un participant existant
        /// </summary>
        public void UpdateParticipant(Participant participant)
        {
            var existingParticipant = _participants.FirstOrDefault(p => p.Id == participant.Id);
            if (existingParticipant != null)
            {
                var index = _participants.IndexOf(existingParticipant);
                _participants[index] = participant;
                SaveData();
            }
        }

        /// <summary>
        /// Supprime un participant
        /// </summary>
        public void DeleteParticipant(int id)
        {
            var participant = _participants.FirstOrDefault(p => p.Id == id);
            if (participant != null)
            {
                // Supprimer également les inscriptions liées à ce participant
                var relatedInscriptions = _inscriptions.Where(i => i.ParticipantId == id).ToList();
                foreach (var inscription in relatedInscriptions)
                {
                    _inscriptions.Remove(inscription);
                }

                _participants.Remove(participant);
                SaveData();
            }
        }

        #endregion

        #region Méthodes d'accès aux inscriptions

        /// <summary>
        /// Obtient toutes les inscriptions
        /// </summary>
        public List<Inscription> GetAllInscriptions()
        {
            return _inscriptions.ToList();
        }

        /// <summary>
        /// Obtient les inscriptions pour un atelier spécifique
        /// </summary>
        public List<Inscription> GetInscriptionsByAtelierId(int atelierId)
        {
            return _inscriptions.Where(i => i.AtelierId == atelierId).ToList();
        }

        /// <summary>
        /// Obtient les inscriptions pour un participant spécifique
        /// </summary>
        public List<Inscription> GetInscriptionsByParticipantId(int participantId)
        {
            return _inscriptions.Where(i => i.ParticipantId == participantId).ToList();
        }

        /// <summary>
        /// Obtient une inscription par son ID
        /// </summary>
        public Inscription? GetInscriptionById(int id)
        {
            return _inscriptions.FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Ajoute une nouvelle inscription
        /// </summary>
        public void AddInscription(Inscription inscription)
        {
            // Génère un nouvel ID
            inscription.Id = _inscriptions.Count > 0 ? _inscriptions.Max(i => i.Id) + 1 : 1;
            
            // Établir les relations
            var atelier = GetAtelierById(inscription.AtelierId);
            var participant = GetParticipantById(inscription.ParticipantId);
            
            if (atelier != null && participant != null)
            {
                inscription.Atelier = atelier;
                inscription.Participant = participant;
                
                _inscriptions.Add(inscription);
                atelier.Inscriptions.Add(inscription);
                participant.Inscriptions.Add(inscription);
                
                SaveData();
            }
        }

        /// <summary>
        /// Met à jour une inscription existante
        /// </summary>
        public void UpdateInscription(Inscription inscription)
        {
            var existingInscription = _inscriptions.FirstOrDefault(i => i.Id == inscription.Id);
            if (existingInscription != null)
            {
                var index = _inscriptions.IndexOf(existingInscription);
                _inscriptions[index] = inscription;
                SaveData();
            }
        }

        /// <summary>
        /// Supprime une inscription
        /// </summary>
        public void DeleteInscription(int id)
        {
            var inscription = _inscriptions.FirstOrDefault(i => i.Id == id);
            if (inscription != null)
            {
                _inscriptions.Remove(inscription);
                
                // Mettre à jour les collections liées
                var atelier = GetAtelierById(inscription.AtelierId);
                var participant = GetParticipantById(inscription.ParticipantId);
                
                atelier?.Inscriptions.Remove(inscription);
                participant?.Inscriptions.Remove(inscription);
                
                SaveData();
            }
        }

        #endregion
    }
}
