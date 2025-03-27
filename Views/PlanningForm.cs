using mission.Controllers;
using mission.Models;
using mission.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace mission.Views
{
    /// <summary>
    /// Formulaire d'affichage du planning des ateliers
    /// </summary>
    public class PlanningForm : Form
    {
        private readonly AtelierController _controller;
        private MonthCalendar _calendar = new();
        private FlowLayoutPanel _ateliersPanel = new();
        private ComboBox _cboFiltre = new();
        private Button _btnFermer = new();

        /// <summary>
        /// Constructeur du formulaire de planning
        /// </summary>
        /// <param name="controller">Contrôleur des ateliers</param>
        public PlanningForm(AtelierController controller)
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
            this.Text = "Planning des Ateliers";
            this.Size = new Size(2560, 1440);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 14, FontStyle.Regular);

            // Label pour le filtre
            var lblFiltre = new Label
            {
                Text = "Filtrer par public :",
                Location = new Point(30, 30),
                Width = 180,
                Font = new Font("Segoe UI", 18, FontStyle.Regular)
            };

            // ComboBox pour le filtre
            _cboFiltre.Location = new Point(220, 30);
            _cboFiltre.Width = 400;
            _cboFiltre.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboFiltre.Font = new Font("Segoe UI", 18, FontStyle.Regular);
            _cboFiltre.Height = 40;
            _cboFiltre.Items.Add("Tous les ateliers");
            _cboFiltre.Items.Add("Parents et enfants");
            _cboFiltre.Items.Add("Assistantes maternelles");
            _cboFiltre.SelectedIndex = 0;
            _cboFiltre.SelectedIndexChanged += (s, e) => LoadAteliers();

            // Calendrier
            _calendar.Location = new Point(30, 100);
            _calendar.Width = 550;
            _calendar.Height = 500;
            _calendar.CalendarDimensions = new Size(2, 3);
            _calendar.MaxSelectionCount = 1;
            _calendar.Font = new Font("Segoe UI", 18, FontStyle.Regular);
            _calendar.DateChanged += Calendar_DateChanged;

            // Panel pour afficher les ateliers
            _ateliersPanel.Location = new Point(620, 100);
            _ateliersPanel.Size = new Size(1900, 1200);
            _ateliersPanel.BorderStyle = BorderStyle.FixedSingle;
            _ateliersPanel.AutoScroll = true;
            _ateliersPanel.FlowDirection = FlowDirection.TopDown;
            _ateliersPanel.WrapContents = false;

            // Bouton Fermer
            _btnFermer.Text = "Fermer";
            _btnFermer.Location = new Point(220, 1300);
            _btnFermer.Width = 500;
            _btnFermer.Height = 120;
            _btnFermer.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            _btnFermer.FlatStyle = FlatStyle.Flat;
            _btnFermer.BackColor = AppStyles.SecondaryColor;
            _btnFermer.ForeColor = Color.White;
            _btnFermer.FlatAppearance.BorderSize = 0;
            _btnFermer.Click += (s, e) => this.Close();

            // Ajouter les contrôles au formulaire
            this.Controls.Add(lblFiltre);
            this.Controls.Add(_cboFiltre);
            this.Controls.Add(_calendar);
            this.Controls.Add(_ateliersPanel);
            this.Controls.Add(_btnFermer);
        }

        /// <summary>
        /// Charge les ateliers et les affiche
        /// </summary>
        private void LoadAteliers()
        {
            _ateliersPanel.Controls.Clear();

            var ateliers = _controller.GetAllAteliers();
            
            // Appliquer le filtre par public
            if (_cboFiltre.SelectedIndex > 0)
            {
                TypePublic publicFilter = (TypePublic)(_cboFiltre.SelectedIndex - 1);
                ateliers = ateliers.Where(a => a.PublicConcerne == publicFilter || a.PublicConcerne == TypePublic.Tous).ToList();
            }

            // Filtrer par date si une date est sélectionnée
            if (_calendar.SelectionStart != null)
            {
                DateTime selectedDate = _calendar.SelectionStart.Date;
                ateliers = ateliers.Where(a => a.DateDebut.Date == selectedDate).ToList();
            }

            // Trier par date et heure
            ateliers = ateliers.OrderBy(a => a.DateDebut).ToList();

            if (ateliers.Count == 0)
            {
                var labelNoAteliers = new Label
                {
                    Text = "Aucun atelier trouvé pour cette date.",
                    AutoSize = true,
                    Margin = new Padding(10),
                    Font = new Font("Segoe UI", 16, FontStyle.Regular)
                };
                _ateliersPanel.Controls.Add(labelNoAteliers);
            }
            else
            {
                // Afficher les ateliers
                foreach (var atelier in ateliers)
                {
                    var panel = CreateAtelierPanel(atelier);
                    _ateliersPanel.Controls.Add(panel);
                }
            }
        }

        /// <summary>
        /// Crée un panel pour afficher les informations d'un atelier
        /// </summary>
        private Panel CreateAtelierPanel(Atelier atelier)
        {
            var panel = new Panel
            {
                Width = 1880,
                Height = 200,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(8),
                Padding = new Padding(10)
            };

            // Couleur du panel selon le type de public
            switch (atelier.PublicConcerne)
            {
                case TypePublic.Parents:
                    panel.BackColor = Color.LightBlue;
                    break;
                case TypePublic.AssistantesMaternelles:
                    panel.BackColor = Color.LightGreen;
                    break;
                case TypePublic.Tous:
                    panel.BackColor = Color.LightYellow;
                    break;
            }

            // Créer le label pour le titre
            var labelTitre = new Label
            {
                Text = atelier.Titre,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            // Créer le label pour la date et l'heure
            var labelDate = new Label
            {
                Text = $"Date : {atelier.DateDebut:dd/MM/yyyy} de {atelier.DateDebut:HH:mm} à {atelier.DateFin:HH:mm}",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(15, 60)
            };

            // Créer le label pour le public
            var labelPublic = new Label
            {
                Text = $"Public : {GetPublicDescription(atelier.PublicConcerne)}",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(15, 100)
            };

            // Créer le label pour les places
            var labelPlaces = new Label
            {
                Text = $"Places : {atelier.PlacesDisponibles} / {atelier.NombrePlaces} disponibles",
                Font = new Font("Segoe UI", 18, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(15, 140)
            };

            // Ajouter un label spécial si l'atelier est complet
            if (atelier.EstComplet)
            {
                var labelComplet = new Label
                {
                    Text = "COMPLET",
                    Font = new Font("Segoe UI", 20, FontStyle.Bold),
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(1600, 15)
                };
                panel.Controls.Add(labelComplet);
            }

            // Ajouter les labels au panel
            panel.Controls.Add(labelTitre);
            panel.Controls.Add(labelDate);
            panel.Controls.Add(labelPublic);
            panel.Controls.Add(labelPlaces);

            return panel;
        }

        /// <summary>
        /// Obtient la description du type de public
        /// </summary>
        private string GetPublicDescription(TypePublic publicType)
        {
            return publicType switch
            {
                TypePublic.Parents => "Parents et enfants",
                TypePublic.AssistantesMaternelles => "Assistantes maternelles",
                TypePublic.Tous => "Tous publics",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gestionnaire de l'événement DateChanged du calendrier
        /// </summary>
        private void Calendar_DateChanged(object? sender, DateRangeEventArgs e)
        {
            LoadAteliers();
        }
    }
}
