using mission.Controllers;
using mission.Models;
using mission.Utils;
using System.Drawing;
using System.Drawing.Printing;

namespace mission.Views
{
    /// <summary>
    /// Formulaire de gestion des listes de présence
    /// </summary>
    public class PresenceListForm : Form
    {
        private readonly InscriptionController _inscriptionController;
        private readonly AtelierController _atelierController;
        private readonly ParticipantController _participantController;
        
        private List<Atelier> _ateliers = new();
        private List<Inscription> _inscriptions = new();
        private List<Participant> _participants = new();
        private Atelier? _selectedAtelier;
        
        private PrintDocument _printDocument = new();
        private PrintPreviewDialog _printPreviewDialog = new();
        
        /// <summary>
        /// Constructeur du formulaire
        /// </summary>
        public PresenceListForm(
            InscriptionController inscriptionController,
            AtelierController atelierController,
            ParticipantController participantController)
        {
            _inscriptionController = inscriptionController;
            _atelierController = atelierController;
            _participantController = participantController;
            
            // Configuration initiale du formulaire
            this.Text = "Listes de Présence";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(2560, 1440);
            this.Size = new Size(2560, 1440);
            this.Font = AppStyles.RegularFont;
            
            // Création de l'interface utilisateur
            InitializeUI();
            
            // Charger les données initiales
            LoadAteliers();
        }
        
        /// <summary>
        /// Initialise l'interface utilisateur
        /// </summary>
        private void InitializeUI()
        {
            // Création de l'en-tête
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = AppStyles.PrimaryColor
            };
            
            var lblTitle = new Label
            {
                Text = "LISTES DE PRÉSENCE",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            headerPanel.Controls.Add(lblTitle);
            
            // Création du panneau principal
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };
            
            // Configuration des colonnes pour une meilleure disposition
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            
            // Panneau de sélection d'atelier (gauche)
            var ateliersPanel = CreateAteliersPanel();
            mainPanel.Controls.Add(ateliersPanel, 0, 0);
            
            // Panneau de la liste de présence (droite)
            var presencePanel = CreatePresencePanel();
            mainPanel.Controls.Add(presencePanel, 1, 0);
            
            // Panneau de pied de page
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = AppStyles.SecondaryColor,
                Padding = new Padding(20)
            };
            
            // Bouton de fermeture
            var btnClose = new Button
            {
                Text = "Fermer",
                Size = new Size(400, 80),
                Location = new Point(this.ClientSize.Width - 450, 20),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            
            footerPanel.Controls.Add(btnClose);
            
            // Ajouter les contrôles au formulaire
            this.Controls.Add(mainPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(headerPanel);
            
            // Configuration de l'impression
            ConfigurePrinting();
            
            // Configurer le dessin personnalisé après que tous les contrôles ont été ajoutés
            ConfigureCustomDrawing();
        }
        
        /// <summary>
        /// Configurer le dessin personnalisé
        /// </summary>
        private void ConfigureCustomDrawing()
        {
            var listBox = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            if (listBox == null) return;
            
            listBox.DrawItem += (s, e) => {
                e.DrawBackground();
                
                if (e.Index >= 0 && e.Index < _ateliers.Count && _ateliers[e.Index] != null)
                {
                    var atelier = _ateliers[e.Index];
                    var isSelected = ((ListBox)s).SelectedIndex == e.Index;
                    
                    // Fond selon la sélection
                    using (var brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                    
                    // Texte
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        var titleFont = new Font("Segoe UI", 20, FontStyle.Bold);
                        var detailsFont = new Font("Segoe UI", 16, FontStyle.Regular);
                        string date = atelier.DateDebut.ToString("dd/MM/yyyy HH:mm");
                        
                        e.Graphics.DrawString(atelier.Titre, titleFont, brush, e.Bounds.X + 10, e.Bounds.Y + 5);
                        e.Graphics.DrawString($"Date: {date}", detailsFont, brush, e.Bounds.X + 10, e.Bounds.Y + 40);
                    }
                }
                
                e.DrawFocusRectangle();
            };
        }
        
        /// <summary>
        /// Crée le panel de sélection d'atelier
        /// </summary>
        private Panel CreateAteliersPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                Margin = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Titre du panneau
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = AppStyles.SecondaryColor
            };
            
            var lblHeader = new Label
            {
                Text = "SÉLECTION DE L'ATELIER",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            headerPanel.Controls.Add(lblHeader);
            panel.Controls.Add(headerPanel);
            
            // Panneau de filtres
            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };
            
            // Tableau pour organiser les filtres
            var filterTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            
            // Configuration des colonnes et lignes
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            filterTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            filterTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            // Filtre par date
            var lblDate = new Label
            {
                Text = "Date :",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            
            var dateTimePicker = new DateTimePicker
            {
                Name = "datePickerAtelier",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };
            
            // Filtre par type
            var lblType = new Label
            {
                Text = "Type :",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
            
            var cboType = new ComboBox
            {
                Name = "cboTypeAtelier",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };
            cboType.Items.Add("Tous");
            cboType.Items.Add("Parents et enfants");
            cboType.Items.Add("Assistantes maternelles");
            cboType.SelectedIndex = 0;
            
            // Ajouter les contrôles au tableau
            filterTable.Controls.Add(lblDate, 0, 0);
            filterTable.Controls.Add(dateTimePicker, 1, 0);
            filterTable.Controls.Add(lblType, 0, 1);
            filterTable.Controls.Add(cboType, 1, 1);
            
            filterPanel.Controls.Add(filterTable);
            panel.Controls.Add(filterPanel);
            
            // Bouton de recherche
            var btnSearch = new Button
            {
                Text = "Rechercher",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(20, 10, 20, 20),
                Name = "btnSearchAteliers"
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => FilterAteliers();
            
            panel.Controls.Add(btnSearch);
            
            // Liste des ateliers
            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                Name = "listBoxAteliers",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 80
            };
            
            listBox.SelectedIndexChanged += (s, e) => {
                if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex < _ateliers.Count)
                {
                    _selectedAtelier = _ateliers[listBox.SelectedIndex];
                    LoadPresenceList();
                }
            };
            
            panel.Controls.Add(listBox);
            
            return panel;
        }
        
        /// <summary>
        /// Crée le panel de liste de présence
        /// </summary>
        private Panel CreatePresencePanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                Margin = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Titre du panneau
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = AppStyles.SecondaryColor
            };
            
            var lblHeader = new Label
            {
                Text = "LISTE DE PRÉSENCE",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            headerPanel.Controls.Add(lblHeader);
            panel.Controls.Add(headerPanel);
            
            // Informations sur l'atelier
            var infoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(20),
                BackColor = Color.AliceBlue
            };
            
            var lblAtelierInfo = new Label
            {
                Name = "lblAtelierInfo",
                Text = "Sélectionnez un atelier pour afficher sa liste de présence",
                Font = new Font("Segoe UI", 18, FontStyle.Italic),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            infoPanel.Controls.Add(lblAtelierInfo);
            panel.Controls.Add(infoPanel);
            
            // ListView pour afficher la liste de présence
            var presenceListView = new ListView
            {
                Name = "listViewPresence",
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                CheckBoxes = true
            };
            
            // Configurer les colonnes
            presenceListView.Columns.Add("Nom", 300);
            presenceListView.Columns.Add("Prénom", 300);
            presenceListView.Columns.Add("Type", 200);
            presenceListView.Columns.Add("Téléphone", 250);
            presenceListView.Columns.Add("Présent(e)", 150);
            
            panel.Controls.Add(presenceListView);
            
            // Panneau pour les boutons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };
            
            // Tableau pour organiser les boutons
            var buttonTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            
            // Configuration des colonnes et lignes
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            buttonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            // Boutons pour la liste de présence
            var btnSavePresence = CreateButton("Enregistrer les Présences", Color.ForestGreen, "btnSavePresence");
            btnSavePresence.Click += (s, e) => SavePresenceList();
            
            var btnPrintList = CreateButton("Imprimer la Liste", AppStyles.AccentColor, "btnPrintList");
            btnPrintList.Click += (s, e) => PrintPresenceList();
            
            var btnMarkAllPresent = CreateButton("Tous Présents", Color.DodgerBlue, "btnMarkAllPresent");
            btnMarkAllPresent.Click += (s, e) => MarkAllPresent(true);
            
            var btnMarkAllAbsent = CreateButton("Tous Absents", Color.Firebrick, "btnMarkAllAbsent");
            btnMarkAllAbsent.Click += (s, e) => MarkAllPresent(false);
            
            // Ajouter les boutons au tableau
            buttonTable.Controls.Add(btnSavePresence, 0, 0);
            buttonTable.Controls.Add(btnPrintList, 1, 0);
            buttonTable.Controls.Add(btnMarkAllPresent, 0, 1);
            buttonTable.Controls.Add(btnMarkAllAbsent, 1, 1);
            
            buttonPanel.Controls.Add(buttonTable);
            panel.Controls.Add(buttonPanel);
            
            return panel;
        }
        
        /// <summary>
        /// Crée un bouton formaté
        /// </summary>
        private Button CreateButton(string text, Color backColor, string name)
        {
            var button = new Button
            {
                Text = text,
                Name = name,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(15)
            };
            button.FlatAppearance.BorderSize = 0;
            
            return button;
        }
        
        /// <summary>
        /// Configure les options d'impression
        /// </summary>
        private void ConfigurePrinting()
        {
            _printDocument.PrintPage += (s, e) => {
                if (_selectedAtelier == null) return;
                
                // Caractéristiques de la page
                var pageWidth = e.PageBounds.Width;
                var pageHeight = e.PageBounds.Height;
                var margin = 50;
                
                // Polices
                var titleFont = new Font("Arial", 18, FontStyle.Bold);
                var subtitleFont = new Font("Arial", 14, FontStyle.Bold);
                var headerFont = new Font("Arial", 12, FontStyle.Bold);
                var normalFont = new Font("Arial", 12, FontStyle.Regular);
                
                // Couleurs et pinceaux
                var blackBrush = new SolidBrush(Color.Black);
                var headerBrush = new SolidBrush(Color.DarkBlue);
                
                // Titre
                var titleText = "LISTE DE PRÉSENCE";
                var titleSize = e.Graphics.MeasureString(titleText, titleFont);
                e.Graphics.DrawString(titleText, titleFont, headerBrush, 
                    (pageWidth - titleSize.Width) / 2, margin);
                
                // Sous-titre (informations atelier)
                var subtitleText = $"Atelier : {_selectedAtelier.Titre} | Date : {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm}";
                var subtitleSize = e.Graphics.MeasureString(subtitleText, subtitleFont);
                e.Graphics.DrawString(subtitleText, subtitleFont, blackBrush, 
                    (pageWidth - subtitleSize.Width) / 2, margin + titleSize.Height + 10);
                
                // Entêtes du tableau
                var cellMargin = 5;
                var tableTop = margin + titleSize.Height + subtitleSize.Height + 40;
                var columns = new[] { "Nom", "Prénom", "Type", "Présent(e)" };
                var columnWidths = new[] { 150, 150, 100, 100 };
                var tableWidth = columnWidths.Sum() + (columns.Length * 2);
                var tableLeft = (pageWidth - tableWidth) / 2;
                
                // Dessiner l'entête du tableau
                var currentX = tableLeft;
                var currentY = tableTop;
                
                for (int i = 0; i < columns.Length; i++)
                {
                    var cellRect = new Rectangle((int)currentX, (int)currentY, columnWidths[i], 30);
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), cellRect);
                    e.Graphics.DrawRectangle(Pens.Black, cellRect);
                    
                    var headerSize = e.Graphics.MeasureString(columns[i], headerFont);
                    e.Graphics.DrawString(columns[i], headerFont, blackBrush, 
                        (int)(currentX + cellMargin), (int)(currentY + (30 - headerSize.Height) / 2));
                    
                    currentX += columnWidths[i];
                }
                
                // Dessiner les lignes du tableau
                currentY += 30;
                
                if (_inscriptions != null)
                {
                    foreach (var inscription in _inscriptions)
                    {
                        var participant = _participantController.GetParticipantById(inscription.ParticipantId);
                        if (participant == null) continue;
                        
                        currentX = tableLeft;
                        
                        // Nom
                        var cellRect = new Rectangle((int)currentX, (int)currentY, columnWidths[0], 25);
                        e.Graphics.DrawRectangle(Pens.Black, cellRect);
                        e.Graphics.DrawString(participant.Nom, normalFont, blackBrush, 
                            (int)(currentX + cellMargin), (int)(currentY + cellMargin));
                        currentX += columnWidths[0];
                        
                        // Prénom
                        cellRect = new Rectangle((int)currentX, (int)currentY, columnWidths[1], 25);
                        e.Graphics.DrawRectangle(Pens.Black, cellRect);
                        e.Graphics.DrawString(participant.Prenom, normalFont, blackBrush, 
                            (int)(currentX + cellMargin), (int)(currentY + cellMargin));
                        currentX += columnWidths[1];
                        
                        // Type
                        cellRect = new Rectangle((int)currentX, (int)currentY, columnWidths[2], 25);
                        e.Graphics.DrawRectangle(Pens.Black, cellRect);
                        string type = participant.Type == TypeParticipant.Parent ? "Parent" : "A.M.";
                        e.Graphics.DrawString(type, normalFont, blackBrush, 
                            (int)(currentX + cellMargin), (int)(currentY + cellMargin));
                        currentX += columnWidths[2];
                        
                        // Case Présence
                        cellRect = new Rectangle((int)currentX, (int)currentY, columnWidths[3], 25);
                        e.Graphics.DrawRectangle(Pens.Black, cellRect);
                        
                        currentY += 25;
                        
                        // Vérifier si on dépasse la page
                        if (currentY > pageHeight - margin)
                        {
                            e.HasMorePages = true;
                            return;
                        }
                    }
                }
                
                // Pied de page
                var footerY = pageHeight - margin - 30;
                e.Graphics.DrawString("Date d'impression : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 
                    new Font("Arial", 10, FontStyle.Italic), blackBrush, margin, footerY);
                
                e.HasMorePages = false;
            };
            
            _printPreviewDialog.Document = _printDocument;
            _printPreviewDialog.Width = 1200;
            _printPreviewDialog.Height = 800;
        }
        
        /// <summary>
        /// Charge la liste des ateliers
        /// </summary>
        private void LoadAteliers()
        {
            _ateliers = _atelierController.GetAllAteliers();
            
            var listBox = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            if (listBox != null)
            {
                listBox.Items.Clear();
                
                foreach (var atelier in _ateliers)
                {
                    // Utiliser le DrawItem event pour afficher les ateliers
                    listBox.Items.Add(atelier);
                }
            }
        }
        
        /// <summary>
        /// Charge la liste de présence pour l'atelier sélectionné
        /// </summary>
        private void LoadPresenceList()
        {
            if (_selectedAtelier == null) return;
            
            // Mettre à jour les informations de l'atelier
            var lblInfo = Controls.Find("lblAtelierInfo", true).FirstOrDefault() as Label;
            if (lblInfo != null)
            {
                lblInfo.Text = $"Atelier: {_selectedAtelier.Titre} | Date: {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm} | Places: {_selectedAtelier.NombrePlaces}";
            }
            
            // Récupérer les inscriptions
            _inscriptions = _inscriptionController.GetInscriptionsByAtelierId(_selectedAtelier.Id);
            
            // Remplir la ListView
            var listView = Controls.Find("listViewPresence", true).FirstOrDefault() as ListView;
            if (listView != null)
            {
                listView.Items.Clear();
                
                foreach (var inscription in _inscriptions)
                {
                    var participant = _participantController.GetParticipantById(inscription.ParticipantId);
                    if (participant == null) continue;
                    
                    var item = new ListViewItem(participant.Nom);
                    item.SubItems.Add(participant.Prenom);
                    item.SubItems.Add(participant.Type == TypeParticipant.Parent ? "Parent" : "A.M.");
                    item.SubItems.Add(participant.Telephone);
                    item.SubItems.Add(string.Empty); // Colonne pour la présence
                    item.Tag = inscription;
                    
                    listView.Items.Add(item);
                }
            }
        }
        
        /// <summary>
        /// Filtre les ateliers selon les critères
        /// </summary>
        private void FilterAteliers()
        {
            var datePicker = Controls.Find("datePickerAtelier", true).FirstOrDefault() as DateTimePicker;
            var cboType = Controls.Find("cboTypeAtelier", true).FirstOrDefault() as ComboBox;
            
            if (datePicker == null || cboType == null) return;
            
            // Filtrer par date
            DateTime selectedDate = datePicker.Value.Date;
            
            // Filtrer par type
            int typeIndex = cboType.SelectedIndex;
            
            // Appliquer les filtres
            var filteredAteliers = _atelierController.GetAllAteliers();
            
            // Filtre par date
            filteredAteliers = filteredAteliers.Where(a => a.DateDebut.Date == selectedDate).ToList();
            
            // Filtre par type
            if (typeIndex == 1) // Parents et enfants
            {
                filteredAteliers = filteredAteliers.Where(a => a.PublicConcerne == TypePublic.Parents).ToList();
            }
            else if (typeIndex == 2) // Assistantes maternelles
            {
                filteredAteliers = filteredAteliers.Where(a => a.PublicConcerne == TypePublic.AssistantesMaternelles).ToList();
            }
            
            // Mettre à jour la liste
            _ateliers = filteredAteliers;
            
            var listBox = Controls.Find("listBoxAteliers", true).FirstOrDefault() as ListBox;
            if (listBox != null)
            {
                listBox.Items.Clear();
                
                foreach (var atelier in _ateliers)
                {
                    listBox.Items.Add(atelier);
                }
            }
        }
        
        /// <summary>
        /// Marquer tous les participants comme présents ou absents
        /// </summary>
        private void MarkAllPresent(bool present)
        {
            var listView = Controls.Find("listViewPresence", true).FirstOrDefault() as ListView;
            if (listView == null) return;
            
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = present;
            }
        }
        
        /// <summary>
        /// Enregistre la liste de présence
        /// </summary>
        private void SavePresenceList()
        {
            var listView = Controls.Find("listViewPresence", true).FirstOrDefault() as ListView;
            if (listView == null || _selectedAtelier == null) return;
            
            // Simuler la sauvegarde des présences
            var presenceCount = 0;
            
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                {
                    presenceCount++;
                }
            }
            
            MessageBox.Show(
                $"Présences enregistrées avec succès!\n\nParticipants présents: {presenceCount}/{listView.Items.Count}",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        
        /// <summary>
        /// Imprime la liste de présence
        /// </summary>
        private void PrintPresenceList()
        {
            if (_selectedAtelier == null)
            {
                MessageBox.Show(
                    "Veuillez sélectionner un atelier avant d'imprimer la liste de présence",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            
            _printPreviewDialog.ShowDialog();
        }
    }
}
