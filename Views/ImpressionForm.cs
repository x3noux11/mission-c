using mission.Controllers;
using mission.Models;
using mission.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mission.Views
{
    /// <summary>
    /// Formulaire pour l'impression des listes d'inscrits aux ateliers
    /// </summary>
    public class ImpressionForm : Form
    {
        private readonly AtelierController _atelierController;
        private readonly InscriptionController _inscriptionController;
        
        private List<Atelier> _ateliers = new();
        private Atelier? _selectedAtelier;
        private int _currentPage; // Variable pour suivre la pagination lors de l'impression
        
        private PrintDocument _printDocument = new();
        private PrintPreviewDialog _printPreviewDialog = new();

        /// <summary>
        /// Constructeur du formulaire d'impression
        /// </summary>
        public ImpressionForm(AtelierController atelierController, InscriptionController inscriptionController)
        {
            _atelierController = atelierController;
            _inscriptionController = inscriptionController;
            
            // Configuration initiale du formulaire
            this.Text = "Impression des Listes d'Inscrits";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 750);
            this.WindowState = FormWindowState.Maximized; // Démarrer en plein écran
            
            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);
            
            // Initialiser les contrôles
            InitializeComponents();
            
            // Initialiser le document d'impression et charger les données
            InitializePrintDocument();
            LoadAteliers();
        }

        /// <summary>
        /// Initialise les composants du formulaire
        /// </summary>
        private void InitializeComponents()
        {
            // Configuration initiale du formulaire
            this.Text = "Impression des Listes d'Inscrits";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 750);
            this.WindowState = FormWindowState.Maximized; // Démarrer en plein écran
            
            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);
            
            // Création des composants
            CreateHeaderPanel();
            CreateWorkshopSelectionPanel();
            CreateFooterPanel();
            CreateParticipantListPanel();
            
            // Configuration de l'impression
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
            
            // Connecter les événements
            ConnectEvents();
        }
        
        /// <summary>
        /// Connecte les événements des contrôles
        /// </summary>
        private void ConnectEvents()
        {
            // Récupérer les contrôles par leur nom
            var cboAteliers = this.Controls.Find("_cboAteliers", true).FirstOrDefault() as ComboBox;
            var chkMontrerPresent = this.Controls.Find("_chkMontrerPresent", true).FirstOrDefault() as CheckBox;
            var btnImprimer = this.Controls.Find("_btnImprimer", true).FirstOrDefault() as Button;
            var btnApercu = this.Controls.Find("_btnApercu", true).FirstOrDefault() as Button;
            var btnExporter = this.Controls.Find("_btnExporter", true).FirstOrDefault() as Button;
            
            // Vérifier que les contrôles existent
            if (cboAteliers != null && chkMontrerPresent != null && 
                btnImprimer != null && btnApercu != null && btnExporter != null)
            {
                // Connecter les événements
                cboAteliers.SelectedIndexChanged += (s, e) => LoadInscrits();
                chkMontrerPresent.CheckedChanged += (s, e) => LoadInscrits();
                btnImprimer.Click += BtnImprimer_Click;
                btnApercu.Click += BtnApercu_Click;
                btnExporter.Click += BtnExporter_Click;
            }
        }

        /// <summary>
        /// Crée le panel d'en-tête du formulaire
        /// </summary>
        private void CreateHeaderPanel()
        {
            // Panel d'en-tête
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120, // Hauteur augmentée
                BackColor = Color.FromArgb(52, 152, 219)
            };
            
            // Label pour le titre
            var titleLabel = new Label
            {
                Text = "Impression des Listes d'Inscrits",
                ForeColor = Color.White,
                Font = new Font(AppStyles.HeaderFont.FontFamily, 24, FontStyle.Bold), // Taille augmentée
                AutoSize = true,
                Location = new Point(20, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Top // S'assurer que l'ancrage est correctement défini
            };
            
            // Bouton de fermeture
            var btnFermer = new Button
            {
                Name = "_btnFermer",
                Text = "Fermer",
                Size = new Size(200, 70),
                Font = new Font(AppStyles.ButtonFont.FontFamily, 12, FontStyle.Bold),
                Anchor = AnchorStyles.Right | AnchorStyles.Top, // Ancre à droite
                Location = new Point(this.ClientSize.Width - 220, 25)
            };
            
            // Mise à jour de la position lors du redimensionnement
            this.Resize += (s, e) => 
            {
                btnFermer.Location = new Point(this.ClientSize.Width - 220, 25);
            };
            
            // Gestionnaire d'événement pour le clic sur le bouton de fermeture
            btnFermer.Click += (s, e) => this.Close();
            
            // Ajouter les contrôles au panel
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(btnFermer);
            
            // Ajouter le panel au formulaire
            this.Controls.Add(headerPanel);
        }

        /// <summary>
        /// Crée le panel de sélection de l'atelier
        /// </summary>
        private void CreateWorkshopSelectionPanel()
        {
            // Panel de sélection de l'atelier
            var workshopPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150, // Hauteur augmentée
                Padding = new Padding(20),
                BackColor = Color.White
            };
            
            // Label pour la sélection de l'atelier
            var lblAtelier = new Label
            {
                Text = "Sélectionner un atelier :",
                AutoSize = true,
                Location = new Point(10, 20),
                Font = AppStyles.RegularFont
            };
            
            // ComboBox pour la sélection de l'atelier
            var cboAteliers = new ComboBox
            {
                Name = "_cboAteliers",
                Location = new Point(200, 17),
                Width = 450,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppStyles.RegularFont
            };
            
            // CheckBox pour afficher les présences
            var chkMontrerPresent = new CheckBox
            {
                Name = "_chkMontrerPresent",
                Text = "Indiquer présent/absent",
                AutoSize = true,
                Location = new Point(670, 20),
                Font = AppStyles.RegularFont
            };
            
            // Label pour les informations de l'atelier
            var lblInfoAtelier = new Label
            {
                Name = "_lblInfoAtelier",
                AutoSize = true,
                Location = new Point(200, 50),
                Font = AppStyles.SmallFont,
                ForeColor = Color.Gray
            };
            
            // Ajouter les contrôles au panel
            workshopPanel.Controls.Add(lblAtelier);
            workshopPanel.Controls.Add(cboAteliers);
            workshopPanel.Controls.Add(chkMontrerPresent);
            workshopPanel.Controls.Add(lblInfoAtelier);
            
            // Ajouter le panel au formulaire
            this.Controls.Add(workshopPanel);
        }

        /// <summary>
        /// Crée le panel de pied de page avec les boutons d'action
        /// </summary>
        private void CreateFooterPanel()
        {
            // Panel de pied de page
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                BackColor = Color.WhiteSmoke
            };
            
            // FlowLayoutPanel pour les boutons d'action
            var buttonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Height = 80,
                Width = 650,
                Location = new Point(20, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Top, // S'assurer que l'ancrage est correctement défini
                AutoSize = true
            };
            
            // Bouton d'impression
            var btnImprimer = new Button
            {
                Name = "_btnImprimer",
                Text = "Imprimer",
                Size = new Size(200, 70),
                Margin = new Padding(0, 0, 15, 0),
                Font = new Font(AppStyles.ButtonFont.FontFamily, 12, FontStyle.Bold)
            };
            
            // Bouton d'aperçu
            var btnApercu = new Button
            {
                Name = "_btnApercu",
                Text = "Aperçu",
                Size = new Size(200, 70),
                Margin = new Padding(0, 0, 15, 0),
                Font = new Font(AppStyles.ButtonFont.FontFamily, 12, FontStyle.Bold)
            };
            
            // Bouton d'exportation
            var btnExporter = new Button
            {
                Name = "_btnExporter",
                Text = "Exporter CSV",
                Size = new Size(200, 70),
                Font = new Font(AppStyles.ButtonFont.FontFamily, 12, FontStyle.Bold)
            };
            
            // Bouton de fermeture
            var btnFermer = new Button
            {
                Name = "_btnFermer",
                Text = "Fermer",
                Size = new Size(200, 70),
                Location = new Point(this.ClientSize.Width - 220, 15),
                Font = new Font(AppStyles.ButtonFont.FontFamily, 12, FontStyle.Bold)
            };
            
            // Mise à jour de la position lors du redimensionnement
            this.Resize += (s, e) => 
            {
                btnFermer.Location = new Point(this.ClientSize.Width - 220, 15);
            };
            
            // Appliquer les styles aux boutons
            AppStyles.ApplyButtonStyle(btnImprimer, ButtonStyle.Primary);
            AppStyles.ApplyButtonStyle(btnApercu, ButtonStyle.Secondary);
            AppStyles.ApplyButtonStyle(btnExporter, ButtonStyle.Success);
            AppStyles.ApplyButtonStyle(btnFermer, ButtonStyle.Light);
            
            // Ajouter les boutons au FlowLayoutPanel
            buttonsPanel.Controls.Add(btnImprimer);
            buttonsPanel.Controls.Add(btnApercu);
            buttonsPanel.Controls.Add(btnExporter);
            
            // Ajouter les contrôles au footer
            footerPanel.Controls.Add(buttonsPanel);
            footerPanel.Controls.Add(btnFermer);
            
            // Ajouter le panel au formulaire
            this.Controls.Add(footerPanel);
        }

        /// <summary>
        /// Crée le panel de la liste des participants
        /// </summary>
        private void CreateParticipantListPanel()
        {
            // Panel de la liste des inscrits
            var inscritsPanel = new Panel
            {
                Dock = DockStyle.Fill, // Remplit tout l'espace disponible
                Padding = new Padding(20)
            };
            
            // DataGridView pour les inscrits
            var dataGridView = new DataGridView
            {
                Name = "_dataGridView",
                Dock = DockStyle.Fill, // Remplit tout l'espace disponible
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Les colonnes s'ajustent à la largeur
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowTemplate = { Height = 40 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 240, 240) },
                Font = new Font(AppStyles.RegularFont.FontFamily, 12, FontStyle.Regular)
            };
            
            // Définir les colonnes
            var numeroColumn = new DataGridViewTextBoxColumn
            {
                Name = "Numero",
                HeaderText = "N°",
                Width = 40
            };

            var nomColumn = new DataGridViewTextBoxColumn
            {
                Name = "Nom",
                HeaderText = "Nom",
                Width = 150
            };

            var prenomColumn = new DataGridViewTextBoxColumn
            {
                Name = "Prenom",
                HeaderText = "Prénom",
                Width = 150
            };

            var typeColumn = new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                Width = 120
            };

            var telephoneColumn = new DataGridViewTextBoxColumn
            {
                Name = "Telephone",
                HeaderText = "Téléphone",
                Width = 120
            };

            var dateInscriptionColumn = new DataGridViewTextBoxColumn
            {
                Name = "DateInscription",
                HeaderText = "Date d'inscription",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            };

            var presenceColumn = new DataGridViewTextBoxColumn
            {
                Name = "Presence",
                HeaderText = "Présence",
                Width = 80
            };

            dataGridView.Columns.AddRange(new DataGridViewColumn[] 
            { 
                numeroColumn, 
                nomColumn, 
                prenomColumn, 
                typeColumn, 
                telephoneColumn, 
                dateInscriptionColumn, 
                presenceColumn
            });
            
            // Personnalisation du style du DataGridView
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = AppStyles.SecondaryColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(AppStyles.RegularFont, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            
            // Ajouter le DataGridView au panel
            inscritsPanel.Controls.Add(dataGridView);
            
            // Ajouter le panel au formulaire
            this.Controls.Add(inscritsPanel);
        }

        /// <summary>
        /// Initialise le document d'impression
        /// </summary>
        private void InitializePrintDocument()
        {
            _printDocument.PrintPage += PrintDocument_PrintPage;
            _printDocument.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
            
            _printPreviewDialog.Document = _printDocument;
            _printPreviewDialog.Width = 800;
            _printPreviewDialog.Height = 600;
        }

        /// <summary>
        /// Charge la liste des ateliers
        /// </summary>
        private void LoadAteliers()
        {
            // Charger tous les ateliers
            _ateliers = _atelierController.GetAllAteliers().OrderByDescending(a => a.DateDebut).ToList();
            
            // Mettre à jour la liste
            var cboAteliers = Controls.Find("_cboAteliers", true).FirstOrDefault() as ComboBox;
            if (cboAteliers != null)
            {
                cboAteliers.Items.Clear();
                foreach (var atelier in _ateliers)
                {
                    cboAteliers.Items.Add($"{atelier.DateDebut:dd/MM/yyyy HH:mm} - {atelier.Titre} ({atelier.Inscriptions.Count} inscrits)");
                }
                
                if (cboAteliers.Items.Count > 0)
                {
                    cboAteliers.SelectedIndex = 0;
                }
                else
                {
                    var dataGridView = Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
                    var btnImprimer = Controls.Find("_btnImprimer", true).FirstOrDefault() as Button;
                    var btnApercu = Controls.Find("_btnApercu", true).FirstOrDefault() as Button;
                    var btnExporter = Controls.Find("_btnExporter", true).FirstOrDefault() as Button;
                    
                    if (dataGridView != null)
                    {
                        dataGridView.Rows.Clear();
                    }
                    
                    if (btnImprimer != null) btnImprimer.Enabled = false;
                    if (btnApercu != null) btnApercu.Enabled = false;
                    if (btnExporter != null) btnExporter.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Met à jour la visibilité de la case à cocher des présences
        /// </summary>
        private void UpdatePresenceCheckboxVisibility()
        {
            var chkMontrerPresent = Controls.Find("_chkMontrerPresent", true).FirstOrDefault() as CheckBox;
            if (chkMontrerPresent == null || _selectedAtelier == null)
            {
                if (chkMontrerPresent != null)
                {
                    chkMontrerPresent.Visible = false;
                }
                return;
            }
            
            bool estPassee = _selectedAtelier.DateDebut < DateTime.Now;
            
            chkMontrerPresent.Visible = estPassee;
            if (!estPassee)
            {
                chkMontrerPresent.Checked = false;
            }
            
            // Mettre à jour l'affichage des informations de l'atelier
            var lblInfoAtelier = Controls.Find("_lblInfoAtelier", true).FirstOrDefault() as Label;
            if (lblInfoAtelier != null && _selectedAtelier != null)
            {
                lblInfoAtelier.Text = $"Date : {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm} | " +
                                     $"Inscrits : {_selectedAtelier.Inscriptions.Count}/{_selectedAtelier.NombrePlaces} | " +
                                     $"Public : {_selectedAtelier.PublicConcerne}";
                
                lblInfoAtelier.ForeColor = estPassee ? Color.DarkGreen : Color.DarkOrange;
            }
        }

        /// <summary>
        /// Charge la liste des inscrits pour l'atelier sélectionné
        /// </summary>
        private void LoadInscrits()
        {
            var cboAteliers = Controls.Find("_cboAteliers", true).FirstOrDefault() as ComboBox;
            if (cboAteliers == null || cboAteliers.SelectedIndex < 0)
                return;
                
            // Récupérer l'atelier sélectionné
            _selectedAtelier = _ateliers[cboAteliers.SelectedIndex];
            
            // Mettre à jour la visibilité de la case à cocher des présences
            UpdatePresenceCheckboxVisibility();

            // Récupérer les inscriptions pour l'atelier sélectionné
            var inscriptions = _inscriptionController.GetInscriptionsByAtelierId(_selectedAtelier.Id)
                .OrderBy(i => i.Participant?.NomComplet)
                .ToList();
            
            // Remplir le DataGridView avec les données
            var dataGridView = Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
            var chkMontrerPresent = Controls.Find("_chkMontrerPresent", true).FirstOrDefault() as CheckBox;
            
            if (dataGridView != null)
            {
                dataGridView.Rows.Clear();
                
                int numero = 1;
                foreach (var inscription in inscriptions)
                {
                    var participant = inscription.Participant;
                    if (participant != null)
                    {
                        int rowIndex = dataGridView.Rows.Add();
                        var row = dataGridView.Rows[rowIndex];
                        
                        row.Cells["Numero"].Value = numero++;
                        row.Cells["Nom"].Value = participant.Nom;
                        row.Cells["Prenom"].Value = participant.Prenom;
                        row.Cells["Type"].Value = participant.Type == TypeParticipant.Parent ? "Parent" : "Assistante maternelle";
                        row.Cells["Telephone"].Value = participant.Telephone;
                        row.Cells["DateInscription"].Value = inscription.DateInscription;
                        
                        // Affichage de la présence uniquement si la case est cochée
                        if (chkMontrerPresent != null && chkMontrerPresent.Checked && chkMontrerPresent.Visible)
                        {
                            row.Cells["Presence"].Value = inscription.Presence ? "Présent" : "Absent";
                        }
                        else
                        {
                            row.Cells["Presence"].Value = "";
                        }
                    }
                }
            }
            
            // Activer les boutons si des données sont présentes
            bool hasData = dataGridView != null && dataGridView.Rows.Count > 0;
            var btnImprimer = Controls.Find("_btnImprimer", true).FirstOrDefault() as Button;
            var btnApercu = Controls.Find("_btnApercu", true).FirstOrDefault() as Button;
            var btnExporter = Controls.Find("_btnExporter", true).FirstOrDefault() as Button;
            
            if (btnImprimer != null) btnImprimer.Enabled = hasData;
            if (btnApercu != null) btnApercu.Enabled = hasData;
            if (btnExporter != null) btnExporter.Enabled = hasData;
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Aperçu
        /// </summary>
        private void BtnApercu_Click(object? sender, EventArgs e)
        {
            try
            {
                _printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'aperçu d'impression : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Imprimer
        /// </summary>
        private void BtnImprimer_Click(object? sender, EventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog
                {
                    Document = _printDocument
                };
                
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    _printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'impression : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Exporter
        /// </summary>
        private void BtnExporter_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedAtelier == null)
                    return;
                    
                // Afficher la boîte de dialogue pour l'enregistrement du fichier
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Fichiers CSV (*.csv)|*.csv",
                    Title = "Exporter la liste des inscrits",
                    FileName = $"Liste_Inscrits_{DateTime.Now:yyyy-MM-dd}.csv"
                };
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var dataGridView = Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
                    var chkMontrerPresent = Controls.Find("_chkMontrerPresent", true).FirstOrDefault() as CheckBox;
                    
                    if (dataGridView == null)
                        return;
                    
                    // Récupérer les données
                    var lignes = new List<string>();
                    
                    // Entête du fichier
                    lignes.Add($"Liste des inscrits - {_selectedAtelier.Titre} - {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm}");
                    lignes.Add("");
                    
                    // Entêtes de colonnes
                    var entetes = new List<string> { "N°", "Nom", "Prénom", "Type", "Téléphone", "Date d'inscription" };
                    if (chkMontrerPresent != null && chkMontrerPresent.Checked && chkMontrerPresent.Visible)
                    {
                        entetes.Add("Présence");
                    }
                    lignes.Add(string.Join(";", entetes));
                    
                    // Données
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        var valeurs = new List<string>();
                        
                        // Ajouter les valeurs des colonnes
                        for (int i = 0; i < entetes.Count; i++)
                        {
                            var valeur = row.Cells[i].Value?.ToString() ?? "";
                            valeur = valeur.Replace(";", ","); // Éviter les problèmes avec le délimiteur CSV
                            valeurs.Add(valeur);
                        }
                        
                        lignes.Add(string.Join(";", valeurs));
                    }
                    
                    // Écrire dans le fichier
                    System.IO.File.WriteAllLines(saveFileDialog.FileName, lignes, Encoding.UTF8);
                    
                    MessageBox.Show(
                        $"Exportation réussie vers {saveFileDialog.FileName}",
                        "Succès",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'exportation : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement PrintPage du document d'impression
        /// </summary>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Vérification des paramètres essentiels
            if (e?.Graphics == null || _selectedAtelier == null)
            {
                return;
            }
            
            // Obtenir une référence au DataGridView
            var dataGridView = this.Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
            if (dataGridView == null || dataGridView.Rows.Count == 0)
            {
                return;
            }
            
            // Réinitialiser le compteur de page au début de l'impression
            if (!e.HasMorePages)
            {
                _currentPage = 0;
            }
            
            // Variables locales pour éviter les références null
            Graphics g = e.Graphics;
            Rectangle bounds = e.MarginBounds;
            
            // Préparation des polices - TAILLES AUGMENTÉES pour plus de lisibilité
            Font titleFont = new Font("Arial", 20, FontStyle.Bold);
            Font headerFont = new Font("Arial", 16, FontStyle.Bold);
            Font normalFont = new Font("Arial", 14);
            Font smallFont = new Font("Arial", 12);
            
            // Préparation des brosses
            Brush titleBrush = new SolidBrush(Color.FromArgb(52, 152, 219)); // Bleu
            Brush headerBrush = Brushes.Black; // Utilisation d'une ressource statique
            Brush textBrush = Brushes.Black;   // Utilisation d'une ressource statique
            Pen linePen = new Pen(Color.Gray, 1);
            
            try
            {
                // Mesurer les hauteurs des éléments
                int titleHeight = (int)g.MeasureString("Test", titleFont).Height;
                int headerHeight = (int)g.MeasureString("Test", headerFont).Height;
                int rowHeight = (int)g.MeasureString("Test", normalFont).Height + 5;
                
                // Position Y courante
                int currentY = bounds.Top;
                
                // Dessiner le titre
                g.DrawString($"Liste des inscrits - {_selectedAtelier.Titre}", 
                    titleFont, titleBrush, bounds.Left, currentY);
                currentY += titleHeight + 10;
                
                // Dessiner les informations de l'atelier
                g.DrawString($"Date: {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm}   Places: {_selectedAtelier.Inscriptions.Count}/{_selectedAtelier.NombrePlaces}   Public: {_selectedAtelier.PublicConcerne}", 
                    smallFont, textBrush, bounds.Left, currentY);
                currentY += headerHeight;
                
                // Dessiner une ligne de séparation
                g.DrawLine(linePen, bounds.Left, currentY, bounds.Right, currentY);
                currentY += 10;
                
                // Définir les en-têtes du tableau
                string[] headers = { "Nom", "Prénom", "Date de naissance", "Téléphone", "Présent" };
                
                // Calculer les largeurs de colonnes
                int[] columnWidths = new int[headers.Length];
                int totalWidth = bounds.Width;
                
                // Distribution proportionnelle des largeurs
                columnWidths[0] = (int)(totalWidth * 0.25); // Nom - 25%
                columnWidths[1] = (int)(totalWidth * 0.25); // Prénom - 25%
                columnWidths[2] = (int)(totalWidth * 0.2);  // Date de naissance - 20%
                columnWidths[3] = (int)(totalWidth * 0.2);  // Téléphone - 20%
                columnWidths[4] = (int)(totalWidth * 0.1);  // Présent - 10%
                
                // Calculer les positions de colonnes
                int[] columnPositions = new int[headers.Length];
                columnPositions[0] = bounds.Left;
                for (int i = 1; i < headers.Length; i++)
                {
                    columnPositions[i] = columnPositions[i - 1] + columnWidths[i - 1];
                }
                
                // Dessiner les en-têtes
                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], headerFont, headerBrush, columnPositions[i], currentY);
                }
                currentY += headerHeight + 5;
                
                // Dessiner une ligne après les en-têtes
                g.DrawLine(linePen, bounds.Left, currentY, bounds.Right, currentY);
                currentY += 5;
                
                // Dessiner les lignes de données
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // Vérifier si on a atteint la fin de la page
                    if (currentY + rowHeight > bounds.Bottom)
                    {
                        e.HasMorePages = true;
                        break; // Sortir de la boucle pour continuer sur la page suivante
                    }
                    
                    // Dessiner les cellules
                    for (int i = 0; i < headers.Length && i < row.Cells.Count; i++)
                    {
                        // Utilisation sécurisée de la valeur de la cellule
                        string cellValue = row.Cells[i].Value?.ToString() ?? string.Empty;
                        g.DrawString(cellValue, normalFont, textBrush, columnPositions[i], currentY);
                    }
                    
                    currentY += rowHeight;
                }
                
                // Si nous avons fini d'imprimer toutes les lignes
                if (!e.HasMorePages)
                {
                    // Dessiner une ligne après les données
                    g.DrawLine(linePen, bounds.Left, currentY, bounds.Right, currentY);
                    currentY += 15;
                    
                    // Dessiner les informations de pagination
                    string pageInfo = $"Page {_currentPage + 1}";
                    SizeF pageInfoSize = g.MeasureString(pageInfo, smallFont);
                    g.DrawString(pageInfo, smallFont, textBrush, 
                        bounds.Right - pageInfoSize.Width, bounds.Bottom - pageInfoSize.Height);
                    
                    // Dessiner la date d'impression
                    string printDate = $"Imprimé le {DateTime.Now:dd/MM/yyyy}";
                    g.DrawString(printDate, smallFont, textBrush, 
                        bounds.Left, bounds.Bottom - pageInfoSize.Height);
                }
                
                // Incrémenter le numéro de page
                _currentPage++;
            }
            finally
            {
                // Libération des ressources allouées
                titleFont.Dispose();
                headerFont.Dispose();
                normalFont.Dispose();
                smallFont.Dispose();
                titleBrush.Dispose();
                linePen.Dispose();
                // Pas besoin de libérer headerBrush et textBrush car ce sont des ressources statiques (Brushes.Black)
            }
        }
    }
}
