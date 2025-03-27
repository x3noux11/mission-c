using mission.Controllers;
using mission.Models;
using mission.Utils;
using System.Drawing;

namespace mission.Views
{
    /// <summary>
    /// Formulaire de gestion des participants
    /// </summary>
    public class ParticipantForm : Form
    {
        private readonly ParticipantController _controller;
        private List<Participant> _participants = new();
        private Participant? _selectedParticipant;
        
        /// <summary>
        /// Constructeur du formulaire
        /// </summary>
        /// <param name="controller">Contrôleur de participants</param>
        public ParticipantForm(ParticipantController controller)
        {
            _controller = controller;
            
            // Configuration initiale du formulaire
            this.Text = "Gestion des Participants";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(2560, 1440);
            this.Size = new Size(2560, 1440);
            
            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);
            
            // Initialiser les contrôles
            InitializeControls();
            
            // Charger les données
            RefreshParticipantsList();
        }
        
        /// <summary>
        /// Initialise les contrôles du formulaire
        /// </summary>
        private void InitializeControls()
        {
            // Création d'un en-tête
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = AppStyles.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "Gestion des Participants",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            headerPanel.Controls.Add(lblTitle);

            // Création d'un TableLayoutPanel principal avec 2 colonnes et 2 lignes
            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(20)
            };

            // Configuration des colonnes et lignes
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Panel pour les assistantes maternelles (en haut à gauche)
            var amPanel = CreateParticipantPanel("Assistantes Maternelles");
            mainTable.Controls.Add(amPanel, 0, 0);

            // Panel pour les enfants (en haut à droite)
            var enfantsPanel = CreateParticipantPanel("Enfants");
            mainTable.Controls.Add(enfantsPanel, 1, 0);

            // Panel pour les détails (en bas, couvrant les deux colonnes)
            var detailsPanel = CreateDetailsPanel();
            mainTable.SetColumnSpan(detailsPanel, 2);
            mainTable.Controls.Add(detailsPanel, 0, 1);

            // Panel pour le pied de page
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = AppStyles.SecondaryColor,
                Padding = new Padding(20)
            };

            // Bouton Fermer
            var btnFermer = new Button
            {
                Text = "Fermer",
                Size = new Size(400, 80),
                Location = new Point(this.ClientSize.Width - 450, 20),
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFermer.FlatAppearance.BorderSize = 0;
            btnFermer.Click += (s, e) => this.Close();

            footerPanel.Controls.Add(btnFermer);

            // Ajout des contrôles au formulaire
            this.Controls.Add(mainTable);
            this.Controls.Add(footerPanel);
            this.Controls.Add(headerPanel);
        }
        
        /// <summary>
        /// Crée un panel pour la liste des participants avec ses boutons
        /// </summary>
        private Panel CreateParticipantPanel(string title)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            // Titre
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = AppStyles.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            headerPanel.Controls.Add(lblTitle);
            panel.Controls.Add(headerPanel);
            
            // Panel de filtrage
            var filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.AliceBlue,
                Padding = new Padding(10)
            };
            
            // Créer un TableLayoutPanel pour les boutons de filtrage
            var filterButtons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            
            // Configuration des colonnes
            filterButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            filterButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            filterButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            
            // Boutons de filtrage avec des tailles plus grandes
            var btnTous = new Button
            {
                Text = "TOUS",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                Name = title == "Assistantes Maternelles" ? "btnTousAM" : "btnTousEnfants"
            };
            btnTous.FlatAppearance.BorderSize = 0;
            
            var btnActifs = new Button
            {
                Text = "ACTIFS",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Name = title == "Assistantes Maternelles" ? "btnActifsAM" : "btnActifsEnfants"
            };
            btnActifs.FlatAppearance.BorderSize = 0;
            
            var btnInactifs = new Button
            {
                Text = "INACTIFS",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5),
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                Name = title == "Assistantes Maternelles" ? "btnInactifsAM" : "btnInactifsEnfants"
            };
            btnInactifs.FlatAppearance.BorderSize = 0;
            
            // Ajouter les gestionnaires d'événements
            btnTous.Click += (sender, e) => FilterParticipants(title, "tous");
            btnActifs.Click += (sender, e) => FilterParticipants(title, "actifs");
            btnInactifs.Click += (sender, e) => FilterParticipants(title, "inactifs");
            
            // Ajouter les boutons au tableau
            filterButtons.Controls.Add(btnTous, 0, 0);
            filterButtons.Controls.Add(btnActifs, 1, 0);
            filterButtons.Controls.Add(btnInactifs, 2, 0);
            
            filterPanel.Controls.Add(filterButtons);
            panel.Controls.Add(filterPanel);

            // Liste
            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                Name = title == "Assistantes Maternelles" ? "listBoxAM" : "listBoxEnfants",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 60
            };
            
            // Configurer le dessin personnalisé pour les éléments de la liste
            ConfigureListDrawing(listBox, title);
            
            listBox.SelectedIndexChanged += (sender, e) => {
                if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex < _participants.Count)
                {
                    _selectedParticipant = _participants[listBox.SelectedIndex];
                    DisplayParticipantDetails();
                }
            };
            
            panel.Controls.Add(listBox);

            // Panel de boutons
            var buttonsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            // Configuration des colonnes
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Boutons
            var btnAdd = CreateButton("Ajouter", AppStyles.AccentColor);
            var btnEdit = CreateButton("Modifier", AppStyles.SecondaryColor);
            var btnDelete = CreateButton("Supprimer", Color.Firebrick);
            
            // Définir les noms des boutons
            btnAdd.Name = title == "Assistantes Maternelles" ? "btnAddAM" : "btnAddEnfant";
            btnEdit.Name = title == "Assistantes Maternelles" ? "btnEditAM" : "btnEditEnfant";
            btnDelete.Name = title == "Assistantes Maternelles" ? "btnDeleteAM" : "btnDeleteEnfant";
            
            // Associer les gestionnaires d'événements
            if (title == "Assistantes Maternelles")
            {
                btnAdd.Click += BtnAddAM_Click;
                btnEdit.Click += BtnEditAM_Click;
                btnDelete.Click += BtnDeleteAM_Click;
            }
            else
            {
                btnAdd.Click += BtnAddEnfant_Click;
                btnEdit.Click += BtnEditEnfant_Click;
                btnDelete.Click += BtnDeleteEnfant_Click;
            }

            buttonsPanel.Controls.Add(btnAdd, 0, 0);
            buttonsPanel.Controls.Add(btnEdit, 1, 0);
            buttonsPanel.Controls.Add(btnDelete, 2, 0);

            panel.Controls.Add(buttonsPanel);

            return panel;
        }

        /// <summary>
        /// Configurer le dessin personnalisé pour les éléments de la liste
        /// </summary>
        private void ConfigureListDrawing(ListBox listBox, string title)
        {
            if (listBox == null) return;
            
            listBox.DrawItem += (s, e) => {
                e.DrawBackground();
                
                if (e.Index >= 0 && _participants != null && e.Index < _participants.Count && _participants[e.Index] != null)
                {
                    var participant = _participants[e.Index];
                    if ((title == "Assistantes Maternelles" && participant.Type == TypeParticipant.AssistanteMaternelle) ||
                        (title == "Enfants" && participant.Type == TypeParticipant.Parent))
                    {
                        var isSelected = ((ListBox)s).SelectedIndex == e.Index;
                        
                        // Dessin du fond selon la sélection
                        using (var brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
                        {
                            e.Graphics?.FillRectangle(brush, e.Bounds);
                        }
                        
                        // Dessin du texte
                        using (var brush = new SolidBrush(Color.Black))
                        {
                            var nameFont = new Font("Segoe UI", 18, FontStyle.Bold);
                            var detailsFont = new Font("Segoe UI", 14, FontStyle.Regular);
                            
                            e.Graphics?.DrawString($"{participant.Nom} {participant.Prenom}", nameFont, brush, e.Bounds.X + 10, e.Bounds.Y + 5);
                            e.Graphics?.DrawString($"Tél: {participant.Telephone}", detailsFont, brush, e.Bounds.X + 10, e.Bounds.Y + 35);
                        }
                    }
                }
                
                e.DrawFocusRectangle();
            };
        }
        
        /// <summary>
        /// Crée un panel pour les détails du participant
        /// </summary>
        private Panel CreateDetailsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            // Titre
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = AppStyles.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "Détails du Participant",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            headerPanel.Controls.Add(lblTitle);
            panel.Controls.Add(headerPanel);

            // Champs de détails
            var detailsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(20)
            };

            // Configuration des colonnes
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            detailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Configuration des lignes
            for (int i = 0; i < 5; i++)
            {
                detailsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            }

            // Étiquettes et champs
            AddLabelAndField(detailsTable, "Nom :", "txtNom", 0);
            AddLabelAndField(detailsTable, "Prénom :", "txtPrenom", 1);
            AddLabelAndField(detailsTable, "Téléphone :", "txtTelephone", 2);
            AddLabelAndField(detailsTable, "Email :", "txtEmail", 3);

            // Type de participant (ComboBox)
            var lblType = new Label
            {
                Text = "Type :",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            var cboType = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cboType"
            };
            cboType.Items.Add("Parent");
            cboType.Items.Add("Assistante Maternelle");
            cboType.SelectedIndex = 0;

            detailsTable.Controls.Add(lblType, 0, 4);
            detailsTable.Controls.Add(cboType, 1, 4);

            panel.Controls.Add(detailsTable);

            // Boutons d'action
            var buttonsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 100,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.WhiteSmoke
            };

            // Configuration des colonnes
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Boutons
            var btnSave = CreateButton("Enregistrer", Color.ForestGreen);
            btnSave.Name = "btnSave";
            btnSave.Click += (sender, e) => SaveParticipant();
            
            var btnCancel = CreateButton("Annuler", Color.DarkGray);
            btnCancel.Name = "btnCancel";
            btnCancel.Click += (sender, e) => CancelEdit();

            buttonsPanel.Controls.Add(btnSave, 0, 0);
            buttonsPanel.Controls.Add(btnCancel, 1, 0);

            panel.Controls.Add(buttonsPanel);

            return panel;
        }

        /// <summary>
        /// Crée un bouton formaté
        /// </summary>
        private Button CreateButton(string text, Color backColor)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        /// <summary>
        /// Ajoute une étiquette et un champ texte au TableLayoutPanel
        /// </summary>
        private void AddLabelAndField(TableLayoutPanel table, string labelText, string fieldName, int row)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                Name = fieldName
            };

            table.Controls.Add(label, 0, row);
            table.Controls.Add(textBox, 1, row);
        }
        
        /// <summary>
        /// Rafraîchit la liste des participants
        /// </summary>
        private void RefreshParticipantsList()
        {
            // Récupérer tous les participants
            _participants = _controller.GetAllParticipants();
            
            // Filtrer par défaut avec "tous"
            FilterParticipants("Assistantes Maternelles", "tous");
            FilterParticipants("Enfants", "tous");
        }
        
        /// <summary>
        /// Filtre les participants selon le type et le critère
        /// </summary>
        private void FilterParticipants(string type, string critere)
        {
            var listBox = Controls.Find(type == "Assistantes Maternelles" ? "listBoxAM" : "listBoxEnfants", true).FirstOrDefault() as ListBox;
            if (listBox == null) return;
            
            listBox.Items.Clear();
            
            var typeParticipant = type == "Assistantes Maternelles" ? TypeParticipant.AssistanteMaternelle : TypeParticipant.Parent;
            
            var filteredParticipants = _participants.Where(p => p.Type == typeParticipant).ToList();
            
            // On simule juste le filtrage actif/inactif pour démonstration
            if (critere == "actifs" || critere == "inactifs")
            {
                // Dans une vraie application, on utiliserait un champ de statut pour filtrer
                // Pour la démo, on considère que les participants avec un email sont "actifs"
                bool isActive = critere == "actifs";
                filteredParticipants = filteredParticipants.Where(p => !string.IsNullOrEmpty(p.Email) == isActive).ToList();
            }
            
            // Mise à jour de la liste filtrée
            foreach (var participant in filteredParticipants)
            {
                listBox.Items.Add(participant); // On ajoute l'objet pour le retrouver facilement ensuite
            }
            
            // Mettre en évidence le bouton sélectionné
            HighlightSelectedFilterButton(type, critere);
        }
        
        /// <summary>
        /// Met en évidence le bouton de filtre sélectionné
        /// </summary>
        private void HighlightSelectedFilterButton(string type, string critere)
        {
            // Récupérer tous les boutons de filtre
            var btnTous = Controls.Find(type == "Assistantes Maternelles" ? "btnTousAM" : "btnTousEnfants", true).FirstOrDefault() as Button;
            var btnActifs = Controls.Find(type == "Assistantes Maternelles" ? "btnActifsAM" : "btnActifsEnfants", true).FirstOrDefault() as Button;
            var btnInactifs = Controls.Find(type == "Assistantes Maternelles" ? "btnInactifsAM" : "btnInactifsEnfants", true).FirstOrDefault() as Button;
            
            // Réinitialiser les couleurs
            if (btnTous != null) btnTous.BackColor = AppStyles.AccentColor;
            if (btnActifs != null) btnActifs.BackColor = Color.SteelBlue;
            if (btnInactifs != null) btnInactifs.BackColor = Color.DimGray;
            
            // Mettre en évidence le bouton sélectionné
            if (critere == "tous" && btnTous != null)
            {
                btnTous.BackColor = Color.Orange;
            }
            else if (critere == "actifs" && btnActifs != null)
            {
                btnActifs.BackColor = Color.Orange;
            }
            else if (critere == "inactifs" && btnInactifs != null)
            {
                btnInactifs.BackColor = Color.Orange;
            }
        }
        
        /// <summary>
        /// Affiche les détails du participant sélectionné
        /// </summary>
        private void DisplayParticipantDetails()
        {
            if (_selectedParticipant == null) return;
            
            // Récupérer les contrôles du panneau de détails
            var txtNom = Controls.Find("txtNom", true).FirstOrDefault() as TextBox;
            var txtPrenom = Controls.Find("txtPrenom", true).FirstOrDefault() as TextBox;
            var txtTelephone = Controls.Find("txtTelephone", true).FirstOrDefault() as TextBox;
            var txtEmail = Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
            var cboType = Controls.Find("cboType", true).FirstOrDefault() as ComboBox;
            
            if (txtNom != null) txtNom.Text = _selectedParticipant.Nom;
            if (txtPrenom != null) txtPrenom.Text = _selectedParticipant.Prenom;
            if (txtTelephone != null) txtTelephone.Text = _selectedParticipant.Telephone;
            if (txtEmail != null) txtEmail.Text = _selectedParticipant.Email;
            
            if (cboType != null)
            {
                cboType.SelectedIndex = _selectedParticipant.Type == TypeParticipant.Parent ? 0 : 1;
            }
            
            // Mettre à jour l'état des boutons
            UpdateButtonsState();
        }
        
        /// <summary>
        /// Met à jour l'état des boutons en fonction de la sélection
        /// </summary>
        private void UpdateButtonsState()
        {
            bool hasSelection = _selectedParticipant != null;
            
            // Boutons pour les assistantes maternelles
            var btnEditAM = Controls.Find("btnEditAM", true).FirstOrDefault() as Button;
            var btnDeleteAM = Controls.Find("btnDeleteAM", true).FirstOrDefault() as Button;
            
            // Boutons pour les enfants
            var btnEditEnfant = Controls.Find("btnEditEnfant", true).FirstOrDefault() as Button;
            var btnDeleteEnfant = Controls.Find("btnDeleteEnfant", true).FirstOrDefault() as Button;
            
            if (btnEditAM != null) btnEditAM.Enabled = hasSelection && _selectedParticipant?.Type == TypeParticipant.AssistanteMaternelle;
            if (btnDeleteAM != null) btnDeleteAM.Enabled = hasSelection && _selectedParticipant?.Type == TypeParticipant.AssistanteMaternelle;
            
            if (btnEditEnfant != null) btnEditEnfant.Enabled = hasSelection && _selectedParticipant?.Type == TypeParticipant.Parent;
            if (btnDeleteEnfant != null) btnDeleteEnfant.Enabled = hasSelection && _selectedParticipant?.Type == TypeParticipant.Parent;
        }
        
        /// <summary>
        /// Enregistre les modifications du participant
        /// </summary>
        private void SaveParticipant()
        {
            var txtNom = Controls.Find("txtNom", true).FirstOrDefault() as TextBox;
            var txtPrenom = Controls.Find("txtPrenom", true).FirstOrDefault() as TextBox;
            var txtTelephone = Controls.Find("txtTelephone", true).FirstOrDefault() as TextBox;
            var txtEmail = Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
            var cboType = Controls.Find("cboType", true).FirstOrDefault() as ComboBox;
            
            if (txtNom == null || txtPrenom == null || txtTelephone == null || txtEmail == null || cboType == null)
                return;
            
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text))
            {
                MessageBox.Show("Veuillez remplir au moins le nom et le prénom.", "Erreur de saisie", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Créer ou mettre à jour le participant
            if (_selectedParticipant == null)
            {
                _selectedParticipant = new Participant();
            }
            
            _selectedParticipant.Nom = txtNom.Text;
            _selectedParticipant.Prenom = txtPrenom.Text;
            _selectedParticipant.Telephone = txtTelephone.Text;
            _selectedParticipant.Email = txtEmail.Text;
            _selectedParticipant.Type = cboType.SelectedIndex == 0 ? TypeParticipant.Parent : TypeParticipant.AssistanteMaternelle;
            
            // Enregistrer dans la base de données
            if (_selectedParticipant.Id == 0)
            {
                _controller.AddParticipant(_selectedParticipant);
            }
            else
            {
                _controller.UpdateParticipant(_selectedParticipant);
            }
            
            // Rafraîchir la liste
            RefreshParticipantsList();
            
            // Réinitialiser le formulaire
            ClearFields();
        }
        
        /// <summary>
        /// Annule l'édition en cours
        /// </summary>
        private void CancelEdit()
        {
            ClearFields();
            _selectedParticipant = null;
            UpdateButtonsState();
        }
        
        /// <summary>
        /// Vide les champs du formulaire
        /// </summary>
        private void ClearFields()
        {
            var txtNom = Controls.Find("txtNom", true).FirstOrDefault() as TextBox;
            var txtPrenom = Controls.Find("txtPrenom", true).FirstOrDefault() as TextBox;
            var txtTelephone = Controls.Find("txtTelephone", true).FirstOrDefault() as TextBox;
            var txtEmail = Controls.Find("txtEmail", true).FirstOrDefault() as TextBox;
            var cboType = Controls.Find("cboType", true).FirstOrDefault() as ComboBox;
            
            if (txtNom != null) txtNom.Text = string.Empty;
            if (txtPrenom != null) txtPrenom.Text = string.Empty;
            if (txtTelephone != null) txtTelephone.Text = string.Empty;
            if (txtEmail != null) txtEmail.Text = string.Empty;
            if (cboType != null) cboType.SelectedIndex = 0;
        }
        
        #region Gestionnaires d'événements
        
        /// <summary>
        /// Événement de clic sur le bouton Ajouter AM
        /// </summary>
        private void BtnAddAM_Click(object sender, EventArgs e)
        {
            // Préparer le formulaire pour l'ajout d'une nouvelle assistante maternelle
            ClearFields();
            
            var cboType = Controls.Find("cboType", true).FirstOrDefault() as ComboBox;
            if (cboType != null)
            {
                cboType.SelectedIndex = 1; // Assistante Maternelle
            }
            
            _selectedParticipant = null;
        }
        
        /// <summary>
        /// Événement de clic sur le bouton Modifier AM
        /// </summary>
        private void BtnEditAM_Click(object sender, EventArgs e)
        {
            // Pas besoin de faire quoi que ce soit, DisplayParticipantDetails a déjà chargé les données
        }
        
        /// <summary>
        /// Événement de clic sur le bouton Supprimer AM
        /// </summary>
        private void BtnDeleteAM_Click(object sender, EventArgs e)
        {
            if (_selectedParticipant == null) return;
            
            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer {_selectedParticipant.Prenom} {_selectedParticipant.Nom} ?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                _controller.DeleteParticipant(_selectedParticipant.Id);
                RefreshParticipantsList();
                ClearFields();
                _selectedParticipant = null;
            }
        }
        
        /// <summary>
        /// Événement de clic sur le bouton Ajouter Enfant
        /// </summary>
        private void BtnAddEnfant_Click(object sender, EventArgs e)
        {
            // Préparer le formulaire pour l'ajout d'un nouvel enfant/parent
            ClearFields();
            
            var cboType = Controls.Find("cboType", true).FirstOrDefault() as ComboBox;
            if (cboType != null)
            {
                cboType.SelectedIndex = 0; // Parent/Enfant
            }
            
            _selectedParticipant = null;
        }
        
        /// <summary>
        /// Événement de clic sur le bouton Modifier Enfant
        /// </summary>
        private void BtnEditEnfant_Click(object sender, EventArgs e)
        {
            // Pas besoin de faire quoi que ce soit, DisplayParticipantDetails a déjà chargé les données
        }
        
        /// <summary>
        /// Événement de clic sur le bouton Supprimer Enfant
        /// </summary>
        private void BtnDeleteEnfant_Click(object sender, EventArgs e)
        {
            if (_selectedParticipant == null) return;
            
            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer {_selectedParticipant.Prenom} {_selectedParticipant.Nom} ?",
                "Confirmation de suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                _controller.DeleteParticipant(_selectedParticipant.Id);
                RefreshParticipantsList();
                ClearFields();
                _selectedParticipant = null;
            }
        }
        
        #endregion
    }
}
