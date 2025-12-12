using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Volcano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            AfficheAccueil();
            


        }
        private void AfficheAccueil()
        {
            UCAccueil uc = new UCAccueil(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.butjouer.Click += AfficherChoixPerso;
            uc.butparametre.Click += AfficherParametre;
        }
        private void AfficherParametre(object sender, RoutedEventArgs e)
        {
            UCParametres uc = new UCParametres(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
        }

        private void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPersonnage uc = new UCChoixPersonnage(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.butjouerjeu.Click += afficheJeu;
            uc.btnretour.Click += AfficheAccueil;
        }
      
        private void afficheJeu(object sender, RoutedEventArgs e)
        {
            UCJeux uc = new UCJeux(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
        }
        private void AfficheAccueil(object sender, RoutedEventArgs e)
        {
            UCAccueil uc = new UCAccueil(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.butjouer.Click += AfficherChoixPerso;
        }
       
    }
}