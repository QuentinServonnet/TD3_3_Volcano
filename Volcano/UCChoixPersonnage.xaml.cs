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
    /// <summary>
    /// Interaction logic for UCChoixPersonnage.xaml
    /// </summary>
    public partial class UCChoixPersonnage : UserControl
    {
        public UCChoixPersonnage()
        {
            InitializeComponent();
        }

        private void BtnJouer_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new UCJeux();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UCChoixPersonnage vueRetour = new UCChoixPersonnage();

            // 2. Trouve la fenêtre (Window) qui contient ce UserControl.
            Window parentWindow = Window.GetWindow(this);

            // Vérification de sécurité
            if (parentWindow != null)
            {
                // 3. Change le contenu de la fenêtre parente pour afficher la vue de retour.
                parentWindow.Content = vueRetour;
            }

        }
    }
}
