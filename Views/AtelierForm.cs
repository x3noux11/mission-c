using mission.Controllers;
using mission.Models;
using mission.Utils;
using System;
using System.Windows.Forms;

namespace mission.Views
{
    /// <summary>
    /// Formulaire de gestion des ateliers
    /// </summary>
    public class AtelierForm : Form
    {
        private readonly AtelierController _controller;
        private DataGridView _dataGridView = new();
        private Button _btnAjouter = new();
        private Button _btnModifier = new();
        private Button _btnSupprimer = new();
        private Button _btnFermer = new();
        private Panel _btnPanel = new();

        /// <summary>
        /// Constructeur du formulaire de gestion des ateliers
        /// </summary>
        /// <param name="controller">Contrôleur des ateliers</param>
        public AtelierForm(AtelierController controller)
        {
            _controller = controller;
            InitializeComponent();
            LoadAteliers();
        }

        /// <summary>
        /// Initialise les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "Gestion des Ateliers";
            this.Size = new Size(2560, 1440);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = AppStyles.RegularFont;

            // Création du DataGridView pour les ateliers
            _dataGridView = new DataGridView
            {
                Name = "_dataGridView",
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToOrderColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                RowHeadersWidth = 50,
                RowTemplate = { Height = 40 }
            };

            // En-tête pour le tableau
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = AppStyles.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "Gestion des Ateliers",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            headerPanel.Controls.Add(lblTitle);

            // Configurer les colonnes du DataGridView
            var idColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 50
            };

            var titreColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Titre",
                HeaderText = "Titre",
                Width = 200
            };

            var dateDebutColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DateDebut",
                HeaderText = "Date et heure de début",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            };

            var dureeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Duree",
                HeaderText = "Durée (min)",
                Width = 80
            };

            var placesColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombrePlaces",
                HeaderText = "Places totales",
                Width = 100
            };

            var placesDisponiblesColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PlacesDisponibles",
                HeaderText = "Places disponibles",
                Width = 120
            };

            var publicColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PublicConcerne",
                HeaderText = "Public concerné",
                Width = 150
            };

            _dataGridView.Columns.AddRange(new DataGridViewColumn[] 
            { 
                idColumn, 
                titreColumn, 
                dateDebutColumn, 
                dureeColumn, 
                placesColumn, 
                placesDisponiblesColumn, 
                publicColumn 
            });

            // Panel pour les boutons
            _btnPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 180,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(30)
            };

            // Créer un TableLayoutPanel pour organiser les boutons uniformément
            var buttonTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };

            // Configurer les colonnes du TableLayoutPanel pour qu'elles soient égales
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Bouton d'ajout d'atelier
            _btnAjouter = new Button
            {
                Text = "Ajouter un atelier",
                Size = new Size(500, 120),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20)
            };
            _btnAjouter.FlatAppearance.BorderSize = 0;
            _btnAjouter.Click += BtnAjouter_Click;

            // Bouton de modification d'atelier
            _btnModifier = new Button
            {
                Text = "Modifier",
                Size = new Size(500, 120),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = AppStyles.SecondaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20)
            };
            _btnModifier.FlatAppearance.BorderSize = 0;
            _btnModifier.Click += BtnModifier_Click;

            // Bouton de suppression d'atelier
            _btnSupprimer = new Button
            {
                Text = "Supprimer",
                Size = new Size(500, 120),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20)
            };
            _btnSupprimer.FlatAppearance.BorderSize = 0;
            _btnSupprimer.Click += BtnSupprimer_Click;

            // Bouton de fermeture
            _btnFermer = new Button
            {
                Text = "Fermer",
                Size = new Size(500, 120),
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(20)
            };
            _btnFermer.FlatAppearance.BorderSize = 0;
            _btnFermer.Click += (s, e) => this.Close();

            // Ajouter les boutons au TableLayoutPanel
            buttonTable.Controls.Add(_btnAjouter, 0, 0);
            buttonTable.Controls.Add(_btnModifier, 1, 0);
            buttonTable.Controls.Add(_btnSupprimer, 2, 0);
            buttonTable.Controls.Add(_btnFermer, 3, 0);

            // Ajouter le TableLayoutPanel au panel de boutons
            _btnPanel.Controls.Add(buttonTable);

            // Ajouter les contrôles au formulaire
            this.Controls.Add(_dataGridView);
            this.Controls.Add(_btnPanel);
            this.Controls.Add(headerPanel);
        }

        /// <summary>
        /// Charge la liste des ateliers dans la grille
        /// </summary>
        private void LoadAteliers()
        {
            var ateliers = _controller.GetAllAteliers();
            _dataGridView.DataSource = null;
            _dataGridView.DataSource = ateliers;
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Ajouter
        /// </summary>
        private void BtnAjouter_Click(object? sender, EventArgs e)
        {
            var form = new AtelierEditForm(null, _controller);
            var result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadAteliers();
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Modifier
        /// </summary>
        private void BtnModifier_Click(object? sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un atelier à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = _dataGridView.SelectedRows[0];
            var atelierId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var atelier = _controller.GetAtelierById(atelierId);

            if (atelier != null)
            {
                var form = new AtelierEditForm(atelier, _controller);
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    LoadAteliers();
                }
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Supprimer
        /// </summary>
        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un atelier à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = _dataGridView.SelectedRows[0];
            var atelierId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
            var atelier = _controller.GetAtelierById(atelierId);

            if (atelier != null)
            {
                var confirmation = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'atelier '{atelier.Titre}' ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmation == DialogResult.Yes)
                {
                    try
                    {
                        _controller.DeleteAtelier(atelierId);
                        LoadAteliers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Erreur lors de la suppression : {ex.Message}",
                            "Erreur",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Fermer
        /// </summary>
        private void BtnFermer_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// Formulaire d'édition d'un atelier (ajout/modification)
    /// </summary>
    public class AtelierEditForm : Form
    {
        private readonly Atelier? _atelier;
        private readonly AtelierController _controller;
        
        private TextBox _txtTitre = new();
        private TextBox _txtDescription = new();
        private DateTimePicker _dtpDate = new();
        private DateTimePicker _dtpHeure = new();
        private NumericUpDown _nudDuree = new();
        private NumericUpDown _nudPlaces = new();
        private ComboBox _cboPublic = new();
        private Button _btnEnregistrer = new();
        private Button _btnAnnuler = new();

        /// <summary>
        /// Constructeur du formulaire d'édition d'atelier
        /// </summary>
        /// <param name="atelier">L'atelier à éditer, ou null pour un nouvel atelier</param>
        /// <param name="controller">Le contrôleur des ateliers</param>
        public AtelierEditForm(Atelier? atelier, AtelierController controller)
        {
            _atelier = atelier;
            _controller = controller;
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Initialise les composants du formulaire
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = _atelier == null ? "Ajouter un atelier" : "Modifier un atelier";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Titre
            var lblTitre = new Label
            {
                Text = "Titre :",
                Location = new Point(30, 30),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _txtTitre.Location = new Point(200, 30);
            _txtTitre.Width = 550;
            _txtTitre.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Description
            var lblDescription = new Label
            {
                Text = "Description :",
                Location = new Point(30, 70),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _txtDescription.Location = new Point(200, 70);
            _txtDescription.Width = 550;
            _txtDescription.Height = 120;
            _txtDescription.Multiline = true;
            _txtDescription.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Date
            var lblDate = new Label
            {
                Text = "Date :",
                Location = new Point(30, 200),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _dtpDate.Location = new Point(200, 200);
            _dtpDate.Width = 250;
            _dtpDate.Format = DateTimePickerFormat.Short;
            _dtpDate.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Heure
            var lblHeure = new Label
            {
                Text = "Heure :",
                Location = new Point(30, 240),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _dtpHeure.Location = new Point(200, 240);
            _dtpHeure.Width = 250;
            _dtpHeure.Format = DateTimePickerFormat.Time;
            _dtpHeure.ShowUpDown = true;
            _dtpHeure.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Durée
            var lblDuree = new Label
            {
                Text = "Durée (min) :",
                Location = new Point(30, 280),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _nudDuree.Location = new Point(200, 280);
            _nudDuree.Width = 250;
            _nudDuree.Minimum = 15;
            _nudDuree.Maximum = 480;
            _nudDuree.Increment = 15;
            _nudDuree.Value = 60;
            _nudDuree.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Places
            var lblPlaces = new Label
            {
                Text = "Nb. places :",
                Location = new Point(30, 320),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _nudPlaces.Location = new Point(200, 320);
            _nudPlaces.Width = 250;
            _nudPlaces.Minimum = 1;
            _nudPlaces.Maximum = 100;
            _nudPlaces.Value = 10;
            _nudPlaces.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Public concerné
            var lblPublic = new Label
            {
                Text = "Public :",
                Location = new Point(30, 360),
                Width = 150,
                Font = new Font("Segoe UI", 12, FontStyle.Regular)
            };

            _cboPublic.Location = new Point(200, 360);
            _cboPublic.Width = 250;
            _cboPublic.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboPublic.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // Remplir le combo avec les valeurs de l'enum
            _cboPublic.Items.Add("Parents et enfants");
            _cboPublic.Items.Add("Assistantes maternelles");
            _cboPublic.Items.Add("Tous publics");
            _cboPublic.SelectedIndex = 0;

            // Boutons
            _btnEnregistrer.Text = "Enregistrer";
            _btnEnregistrer.Location = new Point(280, 480);
            _btnEnregistrer.Width = 250;
            _btnEnregistrer.Height = 60;
            _btnEnregistrer.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            _btnEnregistrer.DialogResult = DialogResult.OK;
            _btnEnregistrer.Click += BtnEnregistrer_Click;

            _btnAnnuler.Text = "Annuler";
            _btnAnnuler.Location = new Point(540, 480);
            _btnAnnuler.Width = 250;
            _btnAnnuler.Height = 60;
            _btnAnnuler.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            _btnAnnuler.DialogResult = DialogResult.Cancel;

            // Ajouter les contrôles au formulaire
            this.Controls.Add(lblTitre);
            this.Controls.Add(_txtTitre);
            this.Controls.Add(lblDescription);
            this.Controls.Add(_txtDescription);
            this.Controls.Add(lblDate);
            this.Controls.Add(_dtpDate);
            this.Controls.Add(lblHeure);
            this.Controls.Add(_dtpHeure);
            this.Controls.Add(lblDuree);
            this.Controls.Add(_nudDuree);
            this.Controls.Add(lblPlaces);
            this.Controls.Add(_nudPlaces);
            this.Controls.Add(lblPublic);
            this.Controls.Add(_cboPublic);
            this.Controls.Add(_btnEnregistrer);
            this.Controls.Add(_btnAnnuler);

            this.AcceptButton = _btnEnregistrer;
            this.CancelButton = _btnAnnuler;
        }

        /// <summary>
        /// Charge les données de l'atelier dans le formulaire
        /// </summary>
        private void LoadData()
        {
            if (_atelier != null)
            {
                _txtTitre.Text = _atelier.Titre;
                _txtDescription.Text = _atelier.Description;
                _dtpDate.Value = _atelier.DateDebut.Date;
                _dtpHeure.Value = new DateTime(
                    _atelier.DateDebut.Year,
                    _atelier.DateDebut.Month,
                    _atelier.DateDebut.Day,
                    _atelier.DateDebut.Hour,
                    _atelier.DateDebut.Minute,
                    0);
                _nudDuree.Value = _atelier.Duree;
                _nudPlaces.Value = _atelier.NombrePlaces;
                _cboPublic.SelectedIndex = (int)_atelier.PublicConcerne;
            }
            else
            {
                // Initialiser les valeurs par défaut pour un nouvel atelier
                _dtpDate.Value = DateTime.Today.AddDays(1);
                _dtpHeure.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 9, 0, 0);
            }
        }

        /// <summary>
        /// Gestionnaire de l'événement Click du bouton Enregistrer
        /// </summary>
        private void BtnEnregistrer_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTitre.Text))
            {
                MessageBox.Show("Le titre est obligatoire.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                // Récupérer les valeurs saisies
                var dateDebut = _dtpDate.Value.Date.Add(_dtpHeure.Value.TimeOfDay);
                var duree = (int)_nudDuree.Value;
                var places = (int)_nudPlaces.Value;
                var publicConcerne = (TypePublic)_cboPublic.SelectedIndex;

                if (_atelier == null)
                {
                    // Création d'un nouvel atelier
                    var nouvelAtelier = new Atelier
                    {
                        Titre = _txtTitre.Text,
                        Description = _txtDescription.Text,
                        DateDebut = dateDebut,
                        Duree = duree,
                        NombrePlaces = places,
                        PublicConcerne = publicConcerne
                    };

                    _controller.CreateAtelier(nouvelAtelier);
                }
                else
                {
                    // Modification d'un atelier existant
                    _atelier.Titre = _txtTitre.Text;
                    _atelier.Description = _txtDescription.Text;
                    _atelier.DateDebut = dateDebut;
                    _atelier.Duree = duree;
                    _atelier.NombrePlaces = places;
                    _atelier.PublicConcerne = publicConcerne;

                    _controller.UpdateAtelier(_atelier);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'enregistrement : {ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}
