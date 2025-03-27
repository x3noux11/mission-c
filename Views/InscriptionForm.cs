using mission.Controllers;
using mission.Models;
using mission.Utils;
using System.Drawing;

namespace mission.Views
{
    /// <summary>
    /// Formulaire de gestion des inscriptions aux ateliers
    /// </summary>
    public class InscriptionForm : Form
    {
        private readonly InscriptionController _inscriptionController;
        private readonly AtelierController _atelierController;
        private readonly ParticipantController _participantController;
        
        private List<Atelier> _ateliers = new();
        private List<Participant> _participants = new();
        private List<Inscription> _inscriptions = new();
        private Atelier? _selectedAtelier;
        
        /// <summary>
        /// Constructeur du formulaire
        /// </summary>
        public InscriptionForm(
            InscriptionController inscriptionController,
            AtelierController atelierController,
            ParticipantController participantController)
        {
            _inscriptionController = inscriptionController;
            _atelierController = atelierController;
            _participantController = participantController;
            
            // Configuration initiale du formulaire
            this.Text = "Gestion des Inscriptions";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(2560, 1440);
            this.Size = new Size(2560, 1440);
            this.Font = AppStyles.RegularFont;
            
            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);
            
            // Création de l'en-tête
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = AppStyles.PrimaryColor
            };
            
            var lblTitle = new Label
            {
                Text = "Gestion des Inscriptions aux Ateliers",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            headerPanel.Controls.Add(lblTitle);
            
            // Création du panneau principal avec séparation 40% / 60%
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };
            
            // Configuration des colonnes pour une meilleure distribution de l'espace
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            
            // Panneau pour les ateliers
            var ateliersPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };
            
            // Titre du panneau ateliers
            var ateliersTitlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = AppStyles.SecondaryColor
            };
            
            var lblAteliers = new Label
            {
                Text = "ATELIERS DISPONIBLES",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            ateliersTitlePanel.Controls.Add(lblAteliers);
            ateliersPanel.Controls.Add(ateliersTitlePanel);
            
            // Panneau de filtres pour les ateliers
            var ateliersFilterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10),
                BackColor = Color.WhiteSmoke
            };
            
            var lblAtelierFilter = new Label
            {
                Text = "Filtrer par date :",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(10, 25)
            };
            
            var datePickerAteliers = new DateTimePicker
            {
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Format = DateTimePickerFormat.Short,
                Width = 200,
                Location = new Point(180, 20),
                Name = "datePickerAteliers"
            };
            
            var btnFilterAteliers = new Button
            {
                Text = "Filtrer",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(400, 20),
                Name = "btnFilterAteliers"
            };
            btnFilterAteliers.FlatAppearance.BorderSize = 0;
            
            ateliersFilterPanel.Controls.Add(lblAtelierFilter);
            ateliersFilterPanel.Controls.Add(datePickerAteliers);
            ateliersFilterPanel.Controls.Add(btnFilterAteliers);
            ateliersPanel.Controls.Add(ateliersFilterPanel);
            
            // Liste des ateliers
            var listBoxAteliers = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 20, FontStyle.Regular),
                SelectionMode = SelectionMode.One,
                Name = "listBoxAteliers",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 60
            };
            
            // Configurer le dessin personnalisé pour les éléments de la liste
            listBoxAteliers.DrawItem += (s, e) => {
                e.DrawBackground();
                
                if (e.Index >= 0 && e.Index < _ateliers.Count && _ateliers[e.Index] != null)
                {
                    var atelier = _ateliers[e.Index];
                    var isSelected = ((ListBox)s).SelectedIndex == e.Index;
                    
                    // Dessin du fond selon la sélection
                    using (var brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                    
                    // Dessin du texte
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        var titleFont = new Font("Segoe UI", 18, FontStyle.Bold);
                        var detailsFont = new Font("Segoe UI", 14, FontStyle.Regular);
                        
                        e.Graphics.DrawString(atelier.Titre, titleFont, brush, e.Bounds.X + 10, e.Bounds.Y + 5);
                        e.Graphics.DrawString($"Date: {atelier.DateDebut:dd/MM/yyyy HH:mm}", detailsFont, brush, e.Bounds.X + 10, e.Bounds.Y + 35);
                    }
                }
                
                e.DrawFocusRectangle();
            };
            
            ateliersPanel.Controls.Add(listBoxAteliers);
            
            // Boutons pour les ateliers
            var ateliersButtonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };
            
            // Créer un TableLayoutPanel pour les boutons des ateliers
            var ateliersButtonTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            
            // Configurer les colonnes et lignes du TableLayoutPanel
            ateliersButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ateliersButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ateliersButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            ateliersButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            // Boutons pour gérer les ateliers
            var btnShowInscriptions = new Button
            {
                Text = "Voir les Inscrits",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Name = "btnShowInscriptions"
            };
            btnShowInscriptions.FlatAppearance.BorderSize = 0;
            
            var btnPrint = new Button
            {
                Text = "Imprimer Liste",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = AppStyles.SecondaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Name = "btnPrint"
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            
            var btnPresenceList = new Button
            {
                Text = "Feuille de Présence",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Name = "btnPresenceList"
            };
            btnPresenceList.FlatAppearance.BorderSize = 0;
            
            var btnSendReminders = new Button
            {
                Text = "Rappels",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = Color.Goldenrod,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Name = "btnSendReminders"
            };
            btnSendReminders.FlatAppearance.BorderSize = 0;
            
            // Ajouter les boutons au TableLayoutPanel
            ateliersButtonTable.Controls.Add(btnShowInscriptions, 0, 0);
            ateliersButtonTable.Controls.Add(btnPrint, 1, 0);
            ateliersButtonTable.Controls.Add(btnPresenceList, 0, 1);
            ateliersButtonTable.Controls.Add(btnSendReminders, 1, 1);
            
            // Ajouter le TableLayoutPanel au panel de boutons
            ateliersButtonsPanel.Controls.Add(ateliersButtonTable);
            ateliersPanel.Controls.Add(ateliersButtonsPanel);
            
            // Panneau pour les participants et inscrits
            var participantsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(10)
            };
            
            // Créer un tabControl pour gérer "Participants" et "Inscrits"
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(0, 60),
                SizeMode = TabSizeMode.Fixed
            };
            
            // Créer l'onglet "Participants"
            var tabParticipants = new TabPage("PARTICIPANTS DISPONIBLES")
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold)
            };
            
            // Créer l'onglet "Inscrits"
            var tabInscrits = new TabPage("PARTICIPANTS INSCRITS")
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold)
            };
            
            // Ajouter les onglets au TabControl
            tabControl.Controls.Add(tabParticipants);
            tabControl.Controls.Add(tabInscrits);
            
            // Contenu de l'onglet Participants
            // Groupe de boutons radio pour filtrer les participants
            var radioPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20)
            };
            
            var radioButtonTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            
            // Configurer les colonnes du TableLayoutPanel
            radioButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            radioButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            radioButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            
            // Radio button pour tous les participants
            var radioAll = new RadioButton
            {
                Text = "Tous",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Checked = true,
                Dock = DockStyle.Fill,
                Name = "radioAll"
            };
            
            // Radio button pour les parents
            var radioParents = new RadioButton
            {
                Text = "Parents",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Dock = DockStyle.Fill,
                Name = "radioParents"
            };
            
            // Radio button pour les assistantes maternelles
            var radioAM = new RadioButton
            {
                Text = "Assistantes Maternelles",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Dock = DockStyle.Fill,
                Name = "radioAM"
            };
            
            // Ajouter les radio buttons au TableLayoutPanel
            radioButtonTable.Controls.Add(radioAll, 0, 0);
            radioButtonTable.Controls.Add(radioParents, 1, 0);
            radioButtonTable.Controls.Add(radioAM, 2, 0);
            
            // Ajouter le TableLayoutPanel au panel de radio buttons
            radioPanel.Controls.Add(radioButtonTable);
            tabParticipants.Controls.Add(radioPanel);
            
            // Panneau de recherche
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20)
            };
            
            var searchBox = new TextBox
            {
                Width = 1000,
                Height = 50,
                Location = new Point(20, 15),
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Name = "searchBox",
                PlaceholderText = "Rechercher un participant..."
            };
            
            var searchButton = new Button
            {
                Text = "🔍 Rechercher",
                Width = 300,
                Height = 50,
                Location = new Point(1040, 15),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Name = "searchButton"
            };
            searchButton.FlatAppearance.BorderSize = 0;
            
            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(searchButton);
            tabParticipants.Controls.Add(searchPanel);
            
            // Liste des participants
            var listBoxParticipants = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                SelectionMode = SelectionMode.One,
                Name = "listBoxParticipants",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 50
            };
            
            // Configurer le dessin personnalisé pour les éléments de la liste
            listBoxParticipants.DrawItem += (s, e) => {
                e.DrawBackground();
                
                if (e.Index >= 0 && e.Index < _participants.Count && _participants[e.Index] != null)
                {
                    var participant = _participants[e.Index];
                    var isSelected = ((ListBox)s).SelectedIndex == e.Index;
                    
                    // Dessin du fond selon la sélection
                    using (var brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                    
                    // Dessin du texte
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        var font = new Font("Segoe UI", 18, FontStyle.Regular);
                        string typeText = participant.Type == TypeParticipant.Parent ? "Parent" : "A.M";
                        
                        e.Graphics.DrawString($"{participant.Nom} {participant.Prenom} ({typeText})", font, brush, e.Bounds.X + 10, e.Bounds.Y + 10);
                    }
                }
                
                e.DrawFocusRectangle();
            };
            
            tabParticipants.Controls.Add(listBoxParticipants);
            
            // Boutons pour les participants
            var participantsButtonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };
            
            // Bouton pour ajouter un participant à l'atelier
            var btnAddInscription = new Button
            {
                Text = "Inscrire à l'Atelier Sélectionné",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                BackColor = AppStyles.SuccessColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20),
                Name = "btnAddInscription"
            };
            btnAddInscription.FlatAppearance.BorderSize = 0;
            
            participantsButtonsPanel.Controls.Add(btnAddInscription);
            tabParticipants.Controls.Add(participantsButtonsPanel);
            
            // Contenu de l'onglet Inscrits
            // Liste des participants inscrits
            var listBoxInscrits = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                SelectionMode = SelectionMode.One,
                Name = "listBoxInscrits",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 50
            };
            
            // Configurer le dessin personnalisé pour les éléments de la liste des inscrits
            listBoxInscrits.DrawItem += (s, e) => {
                e.DrawBackground();
                
                if (e.Index >= 0 && _inscriptions != null && e.Index < _inscriptions.Count && _inscriptions[e.Index] != null)
                {
                    var inscription = _inscriptions[e.Index];
                    var participant = _participantController.GetParticipantById(inscription.ParticipantId);
                    
                    if (participant != null)
                    {
                        var isSelected = ((ListBox)s).SelectedIndex == e.Index;
                        
                        // Dessin du fond selon la sélection
                        using (var brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
                        {
                            e.Graphics.FillRectangle(brush, e.Bounds);
                        }
                        
                        // Dessin du texte
                        using (var brush = new SolidBrush(Color.Black))
                        {
                            var font = new Font("Segoe UI", 18, FontStyle.Regular);
                            string typeText = participant.Type == TypeParticipant.Parent ? "Parent" : "A.M";
                            
                            e.Graphics.DrawString($"{participant.Nom} {participant.Prenom} ({typeText})", font, brush, e.Bounds.X + 10, e.Bounds.Y + 10);
                        }
                    }
                }
                
                e.DrawFocusRectangle();
            };
            
            tabInscrits.Controls.Add(listBoxInscrits);
            
            // Boutons pour les inscrits
            var inscritsButtonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };
            
            // Bouton pour supprimer une inscription
            var btnRemoveInscription = new Button
            {
                Text = "Supprimer l'Inscription",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20),
                Name = "btnRemoveInscription"
            };
            btnRemoveInscription.FlatAppearance.BorderSize = 0;
            
            inscritsButtonsPanel.Controls.Add(btnRemoveInscription);
            tabInscrits.Controls.Add(inscritsButtonsPanel);
            
            // Ajouter le TabControl au panel des participants
            participantsPanel.Controls.Add(tabControl);
            
            // Ajouter les panels au TableLayoutPanel principal
            mainPanel.Controls.Add(ateliersPanel, 0, 0);
            mainPanel.Controls.Add(participantsPanel, 1, 0);
            
            // Panel pour le pied de page avec le bouton de fermeture
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Padding = new Padding(30),
                BackColor = AppStyles.SecondaryColor
            };
            
            // Bouton de fermeture
            var btnClose = new Button
            {
                Text = "Fermer",
                Size = new Size(500, 80),
                Location = new Point(this.ClientSize.Width - 600, 20),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            
            footerPanel.Controls.Add(btnClose);
            
            // Ajouter les panels au formulaire
            this.Controls.Add(footerPanel);
            this.Controls.Add(mainPanel);
            this.Controls.Add(headerPanel);
            
            // Initialiser les contrôles
            InitializeControls();
            
            // Charger les données
            RefreshAteliersList();
        }
        
        /// <summary>
        /// Initialise les contrôles du formulaire
        /// </summary>
        private void InitializeControls()
        {
            // Initialiser les événements pour les contrôles
            var listBoxAteliers = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            var listBoxParticipants = Controls.Find("listBoxParticipants", true).FirstOrDefault() as ListBox;
            var listBoxInscrits = Controls.Find("listBoxInscrits", true).FirstOrDefault() as ListBox;
            var btnShowInscriptions = Controls.Find("btnShowInscriptions", true).FirstOrDefault() as Button;
            var btnPrint = Controls.Find("btnPrint", true).FirstOrDefault() as Button;
            var btnAddInscription = Controls.Find("btnAddInscription", true).FirstOrDefault() as Button;
            var btnRemoveInscription = Controls.Find("btnRemoveInscription", true).FirstOrDefault() as Button;
            var searchBox = Controls.Find("searchBox", true).FirstOrDefault() as TextBox;
            var searchButton = Controls.Find("searchButton", true).FirstOrDefault() as Button;
            var datePickerAteliers = Controls.Find("datePickerAteliers", true).FirstOrDefault() as DateTimePicker;
            var btnFilterAteliers = Controls.Find("btnFilterAteliers", true).FirstOrDefault() as Button;
            
            // Configurer les événements ici
            if (listBoxAteliers != null)
                listBoxAteliers.SelectedIndexChanged += (s, e) => SelectAtelier();
                
            if (btnAddInscription != null)
                btnAddInscription.Click += (s, e) => AddInscription();
                
            if (btnRemoveInscription != null)
                btnRemoveInscription.Click += (s, e) => RemoveInscription();
                
            if (btnShowInscriptions != null)
                btnShowInscriptions.Click += (s, e) => ShowInscriptions();
                
            if (btnPrint != null)
                btnPrint.Click += (s, e) => PrintInscriptions();
                
            if (searchButton != null)
                searchButton.Click += (s, e) => SearchParticipants();
                
            if (datePickerAteliers != null)
                datePickerAteliers.ValueChanged += (s, e) => FilterAteliersByDate();
                
            if (btnFilterAteliers != null)
                btnFilterAteliers.Click += (s, e) => FilterAteliersByDate();
        }
        
        /// <summary>
        /// Rafraîchit la liste des ateliers disponibles
        /// </summary>
        private void RefreshAteliersList()
        {
            // Récupérer les ateliers
            _ateliers = _atelierController.GetAllAteliers();
            
            // Mettre à jour la liste des ateliers
            var listBoxAteliers = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            if (listBoxAteliers != null)
            {
                listBoxAteliers.Items.Clear();
                foreach (var atelier in _ateliers)
                {
                    listBoxAteliers.Items.Add($"{atelier.DateDebut:dd/MM/yyyy} - {atelier.Titre}");
                }
            }
        }
        
        /// <summary>
        /// Sélectionne un atelier et met à jour les listes
        /// </summary>
        private void SelectAtelier()
        {
            var listBoxAteliers = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            if (listBoxAteliers != null && listBoxAteliers.SelectedIndex >= 0)
            {
                _selectedAtelier = _ateliers[listBoxAteliers.SelectedIndex];
                RefreshParticipantsList();
            }
        }
        
        /// <summary>
        /// Rafraîchit la liste des participants disponibles
        /// </summary>
        private void RefreshParticipantsList()
        {
            // Récupérer les participants
            _participants = _participantController.GetAllParticipants();
            
            // Mettre à jour la liste des participants
            var listBoxParticipants = Controls.Find("listBoxParticipants", true).FirstOrDefault() as ListBox;
            if (listBoxParticipants != null)
            {
                listBoxParticipants.Items.Clear();
                foreach (var participant in _participants)
                {
                    listBoxParticipants.Items.Add($"{participant.Nom} {participant.Prenom}");
                }
            }
        }
        
        /// <summary>
        /// Ajoute une inscription d'un participant à l'atelier sélectionné
        /// </summary>
        private void AddInscription()
        {
            // Implémentation de l'ajout d'inscription
        }
        
        /// <summary>
        /// Supprime une inscription d'un participant à l'atelier sélectionné
        /// </summary>
        private void RemoveInscription()
        {
            // Implémentation de la suppression d'inscription
        }
        
        /// <summary>
        /// Affiche les inscriptions pour l'atelier sélectionné
        /// </summary>
        private void ShowInscriptions()
        {
            // Implémentation de l'affichage des inscriptions
        }
        
        /// <summary>
        /// Recherche des participants selon un critère
        /// </summary>
        private void SearchParticipants()
        {
            // Implémentation de la recherche
        }
        
        /// <summary>
        /// Imprime la liste des inscriptions pour l'atelier sélectionné
        /// </summary>
        private void PrintInscriptions()
        {
            // Implémentation de l'impression
        }
        
        /// <summary>
        /// Filtre les ateliers par date
        /// </summary>
        private void FilterAteliersByDate()
        {
            // Implémentation du filtrage
        }
    }
}
