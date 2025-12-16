using System.Media;
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
        private SoundPlayer son;

        public MainWindow()
        {
            InitializeComponent();
            InitMusique();


            this.KeyDown += Window_KeyDown;

            //  gere les bouton menu echap
            overlayPause.DemandeReprise += (s, e) => ReprendreJeu();
            overlayPause.DemandeQuitter += (s, e) => QuitterJeu();

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
            uc.btnRetour.Click += AfficheAccueil;
        }

        private void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPersonnage uc = new UCChoixPersonnage(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.butjouerjeu.Click += afficheJeu;
            uc.btnretour.Click += AfficheAccueil;
            uc.butreglejeu.Click += AfficherRegleJeu;
        }
        private void AfficherRegleJeu(object sender, RoutedEventArgs e)
        {
            UCRegleJeu uc = new UCRegleJeu(); // crée et charge l'écran dedémarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.btnretourregle.Click += AfficherChoixPerso;
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
            uc.butparametre.Click += AfficherParametre;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // si echap
            if (e.Key == Key.Escape)
            {
                // si le joueur joue
                if (ZoneJeu.Content is UCJeux jeuEnCours)
                {
                    // on stop le temps
                    jeuEnCours.MettreEnPause();

                    // on affiche le menu pause
                    overlayPause.Visibility = Visibility.Visible;
                }
            }
        }

        private void ReprendreJeu()
        {
            // on cache le menu
            overlayPause.Visibility = Visibility.Collapsed;

            // on relance le timer du jeu
            if (ZoneJeu.Content is UCJeux jeuEnCours)
            {
                jeuEnCours.Reprendre();
            }

            // pour faire marcher les touches ...
            this.Focus();
        }

        private void QuitterJeu()
        {
            overlayPause.Visibility = Visibility.Collapsed;
            AfficheAccueil(); // retour menu
        }
        private static MediaPlayer? musique;
        private void InitMusique()
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets/son JEU.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = 0.5;
            musique.Play();
        }

        private void RelanceMusique(object? sender, EventArgs e)
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
        }

    }
}