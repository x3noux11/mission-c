using mission.Controllers;
using mission.Models;
using mission.Utils;
using mission.Views;
using System.Drawing;

namespace mission
{
    /// <summary>
    /// Formulaire principal de l'application RAM - Gestion des ateliers d'éveil
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly AtelierController _atelierController;
        private readonly ParticipantController _participantController;
        private readonly InscriptionController _inscriptionController;

        /// <summary>
        /// Constructeur du formulaire principal
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Initialiser les contrôleurs
            _atelierController = new AtelierController();
            _participantController = new ParticipantController();
            _inscriptionController = new InscriptionController();

            // Configurer le formulaire
            this.Text = "Relais Assistantes Maternelles - Gestion des Ateliers d'Éveil";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1024, 768);
            this.Font = AppStyles.RegularFont;

            // Appliquer le style du formulaire
            AppStyles.ApplyFormStyle(this);

            // Initialiser l'interface utilisateur
            InitializeUI();
        }

        /// <summary>
        /// Initialise l'interface utilisateur principale avec un design moderne
        /// </summary>
        private void InitializeUI()
        {
            // Créer un panel pour le titre de l'application
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = AppStyles.PrimaryColor
            };

            var lblTitle = new Label
            {
                Text = "Relais Assistantes Maternelles",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = AppStyles.LightTextColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var lblSubtitle = new Label
            {
                Text = "Gestion des Ateliers d'Éveil",
                Font = new Font("Segoe UI", 26, FontStyle.Italic),
                ForeColor = AppStyles.LightTextColor,
                AutoSize = false,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Bottom,
                Height = 50
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);

            // Panel central pour les boutons d'accès rapide
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(80)
            };

            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 3,
                AutoSize = true
            };
            
            // Configuration des proportions des colonnes
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            
            // Configuration des proportions des lignes
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Création des boutons d'accès rapide
            AddFeatureButton(tableLayout, "Gérer les Ateliers", OnGererAteliers, 0, 0);
            AddFeatureButton(tableLayout, "Planning des Ateliers", OnVoirPlanning, 0, 1);
            AddFeatureButton(tableLayout, "Gérer les Participants", OnGererParticipants, 0, 2);
            AddFeatureButton(tableLayout, "Gérer les Inscriptions", OnGererInscriptions, 1, 0);
            AddFeatureButton(tableLayout, "Listes de Présence", OnListesPresence, 1, 1);
            AddFeatureButton(tableLayout, "Imprimer les Listes", OnListeInscrits, 1, 2);

            mainPanel.Controls.Add(tableLayout);

            // Panel pour le pied de page
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = AppStyles.SecondaryColor
            };

            var lblFooter = new Label
            {
                Text = " 2025 Relais Assistantes Maternelles - Version 1.0",
                ForeColor = AppStyles.LightTextColor,
                AutoSize = false,
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            footerPanel.Controls.Add(lblFooter);

            // Ajouter les éléments au formulaire
            this.Controls.Add(mainPanel);
            this.Controls.Add(headerPanel);
            this.Controls.Add(footerPanel);
            
            // Mise à jour de la couleur de fond
            this.BackColor = AppStyles.BackgroundColor;
        }

        /// <summary>
        /// Ajoute un bouton de navigation vers une fonctionnalité
        /// </summary>
        private void AddFeatureButton(TableLayoutPanel panel, string text, EventHandler clickHandler, int row, int column)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(40),
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                BackColor = AppStyles.AccentColor,
                ForeColor = AppStyles.LightTextColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += clickHandler;
            panel.Controls.Add(button, column, row);
        }

        #region Gestionnaires d'événements

        /// <summary>
        /// Ouvre le formulaire de gestion des ateliers
        /// </summary>
        private void OnGererAteliers(object? sender, EventArgs e)
        {
            var form = new AtelierForm(_atelierController);
            form.ShowDialog();
        }

        /// <summary>
        /// Ouvre le formulaire du planning des ateliers
        /// </summary>
        private void OnVoirPlanning(object? sender, EventArgs e)
        {
            var form = new PlanningForm(_atelierController);
            form.ShowDialog();
        }

        /// <summary>
        /// Ouvre le formulaire de gestion des participants
        /// </summary>
        private void OnGererParticipants(object? sender, EventArgs e)
        {
            var form = new ParticipantForm(_participantController);
            form.ShowDialog();
        }

        /// <summary>
        /// Ouvre le formulaire de gestion des inscriptions
        /// </summary>
        private void OnGererInscriptions(object? sender, EventArgs e)
        {
            var form = new InscriptionForm(_inscriptionController, _atelierController, _participantController);
            form.ShowDialog();
        }

        /// <summary>
        /// Ouvre le formulaire des listes de présence
        /// </summary>
        private void OnListesPresence(object? sender, EventArgs e)
        {
            var form = new PresenceListForm(_inscriptionController, _atelierController, _participantController);
            form.ShowDialog();
        }

        /// <summary>
        /// Ouvre le formulaire d'impression des listes d'inscrits
        /// </summary>
        private void OnListeInscrits(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Fonctionnalité d'impression des listes en cours de développement.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion
    }
}
