using System;
using System.Drawing;
using System.Windows.Forms;

namespace mission.Utils
{
    /// <summary>
    /// Classe utilitaire pour définir les styles de l'application
    /// </summary>
    public static class AppStyles
    {
        // Couleurs principales
        public static readonly Color PrimaryColor = Color.FromArgb(0, 123, 255);         // Bleu vif
        public static readonly Color SecondaryColor = Color.FromArgb(0, 82, 204);       // Bleu foncé
        public static readonly Color AccentColor = Color.FromArgb(255, 102, 0);         // Orange vif
        public static readonly Color SuccessColor = Color.FromArgb(40, 167, 69);        // Vert
        public static readonly Color DangerColor = Color.FromArgb(220, 53, 69);         // Rouge vif
        public static readonly Color WarningColor = Color.FromArgb(255, 193, 7);        // Jaune
        public static readonly Color TextColor = Color.FromArgb(0, 0, 0);               // Noir pur
        public static readonly Color LightTextColor = Color.FromArgb(255, 255, 255);    // Blanc pur
        public static readonly Color BackgroundColor = Color.FromArgb(248, 249, 250);   // Blanc cassé
        
        // Police de caractères
        public static readonly Font HeaderFont = new Font("Segoe UI", 28, FontStyle.Bold);
        public static readonly Font SubHeaderFont = new Font("Segoe UI", 24, FontStyle.Regular);
        public static readonly Font RegularFont = new Font("Segoe UI", 18, FontStyle.Regular);
        public static readonly Font SmallFont = new Font("Segoe UI", 16, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 20, FontStyle.Bold);
        
        // Tailles
        public static readonly int ButtonWidth = 400;
        public static readonly int ButtonHeight = 100;
        public static readonly int SmallButtonWidth = 300;
        public static readonly int SmallButtonHeight = 70;
        public static readonly int Padding = 30;
        public static readonly int Margin = 20;
        
        /// <summary>
        /// Configure un formulaire avec le style de l'application
        /// </summary>
        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = RegularFont;
            form.ForeColor = TextColor;
            
            // Si le formulaire a une barre de menu, la styler aussi
            if (form.MainMenuStrip != null)
            {
                form.MainMenuStrip.BackColor = PrimaryColor;
                form.MainMenuStrip.ForeColor = LightTextColor;
                form.MainMenuStrip.Font = RegularFont;
                
                foreach (ToolStripMenuItem item in form.MainMenuStrip.Items)
                {
                    StyleMenuItem(item);
                }
            }
        }
        
        /// <summary>
        /// Style un élément de menu et ses sous-menus
        /// </summary>
        private static void StyleMenuItem(ToolStripMenuItem menuItem)
        {
            menuItem.BackColor = PrimaryColor;
            menuItem.ForeColor = LightTextColor;
            menuItem.Font = RegularFont;
            
            // Styler les sous-menus
            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem subItem)
                {
                    subItem.BackColor = BackgroundColor;
                    subItem.ForeColor = TextColor;
                    subItem.Font = RegularFont;
                    
                    // Récursion pour les sous-menus
                    StyleMenuItem(subItem);
                }
            }
        }
        
        /// <summary>
        /// Configure un bouton avec le style principal
        /// </summary>
        public static void ApplyButtonStyle(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;
            
            switch (style)
            {
                case ButtonStyle.Primary:
                    button.BackColor = PrimaryColor;
                    button.ForeColor = LightTextColor;
                    break;
                case ButtonStyle.Secondary:
                    button.BackColor = SecondaryColor;
                    button.ForeColor = LightTextColor;
                    break;
                case ButtonStyle.Success:
                    button.BackColor = SuccessColor;
                    button.ForeColor = LightTextColor;
                    break;
                case ButtonStyle.Danger:
                    button.BackColor = DangerColor;
                    button.ForeColor = LightTextColor;
                    break;
                case ButtonStyle.Warning:
                    button.BackColor = WarningColor;
                    button.ForeColor = TextColor;
                    break;
                case ButtonStyle.Light:
                    button.BackColor = LightTextColor;
                    button.ForeColor = TextColor;
                    button.FlatAppearance.BorderSize = 1;
                    button.FlatAppearance.BorderColor = Color.LightGray;
                    break;
            }
            
            // Ajout d'un événement pour l'effet hover
            button.MouseEnter += (s, e) => 
            {
                button.BackColor = ChangeColorBrightness(button.BackColor, -0.1f);
            };
            
            button.MouseLeave += (s, e) => 
            {
                button.BackColor = ChangeColorBrightness(button.BackColor, 0.1f);
            };
        }
        
        /// <summary>
        /// Configure un DataGridView avec le style de l'application
        /// </summary>
        public static void ApplyDataGridViewStyle(DataGridView dataGridView)
        {
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.BackgroundColor = BackgroundColor;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = LightTextColor;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(RegularFont, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = SecondaryColor;
            dataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = LightTextColor;
            dataGridView.ColumnHeadersHeight = 35;
            
            dataGridView.DefaultCellStyle.BackColor = BackgroundColor;
            dataGridView.DefaultCellStyle.ForeColor = TextColor;
            dataGridView.DefaultCellStyle.Font = RegularFont;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 236, 241);
            dataGridView.DefaultCellStyle.SelectionForeColor = TextColor;
            
            dataGridView.GridColor = Color.FromArgb(224, 224, 224);
            dataGridView.RowHeadersVisible = false;
            dataGridView.RowTemplate.Height = 30;
            
            // Alternance de couleurs pour les lignes
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }
        
        /// <summary>
        /// Éclaircit ou assombrit une couleur
        /// </summary>
        /// <param name="color">Couleur à modifier</param>
        /// <param name="correctionFactor">Facteur de correction (-1 à 1)</param>
        /// <returns>Couleur modifiée</returns>
        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;
            
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }
    
    /// <summary>
    /// Énumération des styles de boutons disponibles
    /// </summary>
    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Light
    }
}
