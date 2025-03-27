using mission.Controllers;
using mission.Models;
using mission.Utils;
using System.Drawing;

namespace mission.Views
{
    /// <summary>
    /// Formulaire de gestion des présences aux ateliers
    /// </summary>
    public class PresenceForm : Form
    {
        private readonly InscriptionController _inscriptionController;
        private readonly AtelierController _atelierController;
        
        private List<Inscription> _inscriptions = new();
        private List<Atelier> _ateliers = new();
        private Atelier? _selectedAtelier;
        
        /// <summary>
        /// Constructeur du formulaire de gestion des présences
        /// </summary>
        public PresenceForm(InscriptionController inscriptionController, AtelierController atelierController)
        {
            _inscriptionController = inscriptionController;
            _atelierController = atelierController;
            
            // Configuration initiale du formulaire
            this.Text = "Gestion des Présences";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(1000, 700);
            
            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);
            
            // Initialiser les contrôles
            InitializeControls();
            
            // Charger les données
            LoadAteliers();
        }

        /// <summary>
        /// Initialise les composants du formulaire
        /// </summary>
        private void InitializeControls()
        {
            // Panel d'en-tête
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = AppStyles.PrimaryColor
            };
            
            var lblTitle = new Label
            {
                Text = "Gestion des Présences aux Ateliers",
                Font = AppStyles.HeaderFont,
                ForeColor = AppStyles.LightTextColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            headerPanel.Controls.Add(lblTitle);
            
            // Panel principal
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            
            // Panel de sélection d'atelier
            var atelierSelectionPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10)
            };
            
            var lblAtelier = new Label
            {
                Text = "Sélectionner un atelier :",
                AutoSize = true,
                Location = new Point(10, 20),
                Font = AppStyles.RegularFont
            };
            
            var cboAteliers = new ComboBox
            {
                Name = "_cboAteliers",
                Location = new Point(200, 17),
                Width = 450,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = AppStyles.RegularFont
            };
            
            var lblInfoAtelier = new Label
            {
                Name = "_lblInfoAtelier",
                AutoSize = true,
                Location = new Point(200, 50),
                Font = AppStyles.SmallFont,
                ForeColor = Color.Gray
            };
            
            atelierSelectionPanel.Controls.Add(lblAtelier);
            atelierSelectionPanel.Controls.Add(cboAteliers);
            atelierSelectionPanel.Controls.Add(lblInfoAtelier);
            
            // Panel de la liste des présences
            var presencesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            // DataGridView pour les présences
            var dataGridView = new DataGridView
            {
                Name = "_dataGridView",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowTemplate = { Height = 30 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(240, 240, 240) },
                Font = AppStyles.RegularFont
            };
            
            // Définir les colonnes
            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 50,
                ReadOnly = true,
                Visible = false
            };

            var participantColumn = new DataGridViewTextBoxColumn
            {
                Name = "Participant",
                HeaderText = "Participant",
                Width = 200,
                ReadOnly = true
            };

            var typeParticipantColumn = new DataGridViewTextBoxColumn
            {
                Name = "TypeParticipant",
                HeaderText = "Type",
                Width = 150,
                ReadOnly = true
            };

            var dateInscriptionColumn = new DataGridViewTextBoxColumn
            {
                Name = "DateInscription",
                DataPropertyName = "DateInscription",
                HeaderText = "Date d'inscription",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" },
                ReadOnly = true
            };

            var presenceColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Presence",
                DataPropertyName = "Presence",
                HeaderText = "Présent",
                Width = 70
            };

            var commentairesColumn = new DataGridViewTextBoxColumn
            {
                Name = "Commentaires",
                DataPropertyName = "Commentaires",
                HeaderText = "Commentaires",
                Width = 200
            };

            dataGridView.Columns.AddRange(new DataGridViewColumn[] 
            { 
                idColumn, 
                participantColumn, 
                typeParticipantColumn, 
                dateInscriptionColumn, 
                presenceColumn,
                commentairesColumn 
            });
            
            // Personnalisation du style du DataGridView
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = AppStyles.SecondaryColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(AppStyles.RegularFont, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            
            presencesPanel.Controls.Add(dataGridView);
            
            // Panel de pied de page
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.WhiteSmoke
            };
            
            var btnEnregistrer = new Button
            {
                Name = "_btnEnregistrer",
                Text = "Enregistrer les présences",
                Size = new Size(200, 40),
                Location = new Point(20, 15)
            };
            
            var btnFermer = new Button
            {
                Name = "_btnFermer",
                Text = "Fermer",
                Size = new Size(120, 40),
                Location = new Point(this.ClientSize.Width - 150, 15)
            };
            
            // Mise à jour de la position lors du redimensionnement
            this.Resize += (s, e) => 
            {
                btnFermer.Location = new Point(this.ClientSize.Width - 150, 15);
            };
            
            // Appliquer les styles aux boutons
            AppStyles.ApplyButtonStyle(btnEnregistrer, ButtonStyle.Primary);
            AppStyles.ApplyButtonStyle(btnFermer, ButtonStyle.Light);
            
            footerPanel.Controls.Add(btnEnregistrer);
            footerPanel.Controls.Add(btnFermer);
            
            // Ajouter les panels au formulaire
            mainPanel.Controls.Add(presencesPanel);
            mainPanel.Controls.Add(atelierSelectionPanel);
            
            this.Controls.Add(footerPanel);
            this.Controls.Add(mainPanel);
            this.Controls.Add(headerPanel);
            
            // Événements
            cboAteliers.SelectedIndexChanged += OnAtelierSelectionChanged;
            btnEnregistrer.Click += OnEnregistrerClick;
            btnFermer.Click += (s, e) => this.Close();
        }

        /// <summary>
        /// Charge la liste des ateliers
        /// </summary>
        private void LoadAteliers()
        {
            // Charger les ateliers passés et à venir
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
            }
        }

        /// <summary>
        /// Événement de changement d'atelier sélectionné
        /// </summary>
        private void OnAtelierSelectionChanged(object? sender, EventArgs e)
        {
            var cboAteliers = sender as ComboBox;
            if (cboAteliers != null && cboAteliers.SelectedIndex >= 0)
            {
                // Récupérer l'atelier sélectionné
                _selectedAtelier = _ateliers[cboAteliers.SelectedIndex];
                
                // Charger les présences
                LoadPresences();
            }
        }
        
        /// <summary>
        /// Charge la liste des inscriptions pour l'atelier sélectionné
        /// </summary>
        private void LoadPresences()
        {
            if (_selectedAtelier == null)
                return;

            // Récupérer les inscriptions pour l'atelier sélectionné
            _inscriptions = _inscriptionController.GetInscriptionsByAtelierId(_selectedAtelier.Id);
            
            // Mettre à jour la liste des présences
            var dataGridView = Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
            if (dataGridView != null)
            {
                dataGridView.Rows.Clear();
                
                foreach (var inscription in _inscriptions)
                {
                    var participant = inscription.Participant;
                    if (participant != null)
                    {
                        int rowIndex = dataGridView.Rows.Add();
                        var row = dataGridView.Rows[rowIndex];
                        
                        row.Cells["Id"].Value = inscription.Id;
                        row.Cells["Participant"].Value = $"{participant.Nom} {participant.Prenom}";
                        row.Cells["TypeParticipant"].Value = participant.Type == TypeParticipant.Parent ? "Parent" : "Assistante maternelle";
                        row.Cells["DateInscription"].Value = inscription.DateInscription;
                        row.Cells["Presence"].Value = inscription.Presence;
                        row.Cells["Commentaires"].Value = inscription.Commentaires;
                    }
                }
            }
            
            // Mettre à jour l'affichage des informations de l'atelier
            var lblInfoAtelier = Controls.Find("_lblInfoAtelier", true).FirstOrDefault() as Label;
            if (lblInfoAtelier != null && _selectedAtelier != null)
            {
                bool atelierEstPasse = _selectedAtelier.DateDebut < DateTime.Now;
                lblInfoAtelier.Text = $"Date : {_selectedAtelier.DateDebut:dd/MM/yyyy HH:mm} | " +
                                     $"Inscrits : {_inscriptions.Count}/{_selectedAtelier.NombrePlaces} | " +
                                     $"Public : {_selectedAtelier.PublicConcerne}";
                
                lblInfoAtelier.ForeColor = atelierEstPasse ? Color.DarkGreen : Color.DarkOrange;
            }
            
            // Vérifier si l'atelier est passé
            bool estPasse = _selectedAtelier != null && _selectedAtelier.DateDebut < DateTime.Now;
            var btnEnregistrer = Controls.Find("_btnEnregistrer", true).FirstOrDefault() as Button;
            
            if (dataGridView != null && btnEnregistrer != null)
            {
                if (!estPasse)
                {
                    MessageBox.Show(
                        "Cet atelier n'a pas encore eu lieu. Vous pourrez enregistrer les présences après la date de l'atelier.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    
                    dataGridView.ReadOnly = true;
                    btnEnregistrer.Enabled = false;
                }
                else
                {
                    dataGridView.ReadOnly = false;
                    btnEnregistrer.Enabled = true;
                    
                    // Rendre certaines colonnes en lecture seule
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (column.Name != "Presence" && column.Name != "Commentaires")
                        {
                            column.ReadOnly = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Enregistrer
        /// </summary>
        private void OnEnregistrerClick(object? sender, EventArgs e)
        {
            try
            {
                var dataGridView = Controls.Find("_dataGridView", true).FirstOrDefault() as DataGridView;
                if (dataGridView == null)
                    return;
                    
                // Parcourir toutes les lignes du DataGridView
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    int inscriptionId = Convert.ToInt32(row.Cells["Id"].Value);
                    bool presence = Convert.ToBoolean(row.Cells["Presence"].Value);
                    string commentaires = row.Cells["Commentaires"].Value?.ToString() ?? string.Empty;
                    
                    // Récupérer l'inscription
                    var inscription = _inscriptionController.GetInscriptionById(inscriptionId);
                    if (inscription != null)
                    {
                        // Mettre à jour les valeurs
                        inscription.Presence = presence;
                        inscription.Commentaires = commentaires;
                        
                        // Enregistrer les modifications
                        _inscriptionController.UpdateInscription(inscription);
                    }
                }
                
                MessageBox.Show(
                    "Les présences ont été enregistrées avec succès.",
                    "Succès",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'enregistrement des présences : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
