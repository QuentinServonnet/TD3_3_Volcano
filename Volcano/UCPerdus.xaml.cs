using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Volcano
{
    public partial class UCPerdus : UserControl
    {
        // evenemtns pour donner les info a la maiwindow
        public event EventHandler DemandeRejouer;
        public event EventHandler DemandeMenu;

        public UCPerdus()
        {
            InitializeComponent();
        }

        // fonction pour changer le texte quand on perd
        public void SetInfos(string cause, string temps)
        {
            txtCause.Text = cause;
            txtTemps.Text = "Temps de survie : " + temps;
        }

        private void btnRejouer_Click(object sender, RoutedEventArgs e)
        {
            DemandeRejouer?.Invoke(this, EventArgs.Empty);
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            DemandeMenu?.Invoke(this, EventArgs.Empty);
        }
    }
}
