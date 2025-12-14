using System; // Indispensable
using System.Windows;
using System.Windows.Controls;

namespace Volcano
{
    public partial class UCEchap : UserControl
    {
        public event EventHandler DemandeReprise;
        public event EventHandler DemandeQuitter;

        public UCEchap()
        {
            InitializeComponent();
        }

        private void btnReprendre_Click(object sender, RoutedEventArgs e)
        {
            if (DemandeReprise != null)
            {
                DemandeReprise(this, EventArgs.Empty);
            }
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            if (DemandeQuitter != null)
            {
                DemandeQuitter(this, EventArgs.Empty);
            }
        }
    }
}