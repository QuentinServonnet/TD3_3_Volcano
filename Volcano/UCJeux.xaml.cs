using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Volcano
{
    public partial class UCJeux : UserControl
    {
        // VARIABLES

        // graphismes et animation
        private BitmapImage[] persos = new BitmapImage[6];
        private DispatcherTimer minuterie;
        private int nbAnimation = 0;
        private string nomPersonnage;

        // etat du jeu
        private TimeSpan tempsJeu;
        private bool estGameOver = false;

        // mouvements et environnement
        private bool goRight = false;
        private bool goLeft = false;
        private int vitesseFond = 6;
        private const double LARGEUR_IMAGE = 800;

        // lave
        private double vitesseLave = 1.5; // vitesse naturelle de la lave

        //  Météorites
        private List<Image> meteorites = new List<Image>();
        private Random generateurAleatoire = new Random();
        private int vitesseMeteorite = 10;

        // physique
        private bool estEnSaut = false;
        private int force = 0;
        private int gravite = 2;
        private int puissanceSaut = 22;
        private double sol; // La ligne de sol

        //Son
        private static SoundPlayer win;

        // CONSTRUCTEUR

        public UCJeux(string personnageChoisi = "John")
        {
            InitializeComponent();
            this.nomPersonnage = personnageChoisi; // on sauvegarde le choix 

            InitializeImages(); // charge les images du perso
            InitializeTimer();  // lance la boucle de jeu

            tempsJeu = TimeSpan.Zero; // reset du chrono

            this.Loaded += (s, e) =>
            {
                if (Window.GetWindow(this) != null)
                {
                    Window.GetWindow(this).KeyDown += Window_KeyDown;
                    Window.GetWindow(this).KeyUp += Window_KeyUp;
                }
            };

            // on définit le sol selon la position de départ dans le xaml
            sol = Canvas.GetTop(imgPerso);

            // relie le bouton  du menu perdus
            overlayPerdu.DemandeRejouer += (s, e) => ResetPartie();
        }

        // GESTION CLAVIER 

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (estGameOver) return; // bloque les touches si perdu

            if (e.Key == Key.D) goRight = true;
            if (e.Key == Key.Q) goLeft = true;

            // Saut seulement si on est au sol
            if ((e.Key == Key.Z || e.Key == Key.Space) && !estEnSaut)
            {
                estEnSaut = true;
                force = -puissanceSaut;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D) goRight = false;
            if (e.Key == Key.Q) goLeft = false;

            // remettre le perso a l'image 0 quand on ne bouge plus
            if (!goRight && !goLeft && !estEnSaut && !estGameOver)
            {
                if (persos.Length > 0) imgPerso.Source = persos[0];
            }
        }

        // BOUCLE DE JEU
        private void Jeu(object sender, EventArgs e)
        {
            if (estGameOver) return;

            // MISE A JOUR DU CHRONO
            tempsJeu = tempsJeu.Add(minuterie.Interval);
            lblTemps.Content = ((int)tempsJeu.TotalSeconds).ToString() + " s";

            //PHYSIQUE
            if (estEnSaut || Canvas.GetTop(imgPerso) < sol)
            {
                Canvas.SetTop(imgPerso, Canvas.GetTop(imgPerso) + force);
                force += gravite; // la gravité tire vers le bas

                // Atterrissage
                if (Canvas.GetTop(imgPerso) >= sol)
                {
                    Canvas.SetTop(imgPerso, sol);
                    estEnSaut = false;
                    force = 0;
                }
            }

            // ENNEMIS
            BougerLave();
            GererMeteorites();
            VerifierCollisions(); // vérifie si on est mort

            // DÉPLACEMENTS JOUEUR & FOND
            if (goRight)
            {
                imgPerso.RenderTransform = new ScaleTransform(1, 1); // regarde à droite
                BougerFond(-vitesseFond);
                AnimerPerso();
            }
            else if (goLeft)
            {
                imgPerso.RenderTransform = new ScaleTransform(-1, 1); // regarde à gauche
                BougerFond(vitesseFond);
                AnimerPerso();
            }
        }

        // ENNEMIS

        private void BougerLave()
        {
            double posLave = Canvas.GetLeft(img_lave);
            double deplacement = vitesseLave; // la lave avance toujours un peu

            // si le joueur court, la lave recule (car la caméra avance)
            if (goRight) deplacement -= vitesseFond;
            // Si le joueur recule , la lave se rapproche très vite
            else if (goLeft) deplacement += vitesseFond;

            Canvas.SetLeft(img_lave, posLave + deplacement);
        }

        private void GererMeteorites()
        {
            if (generateurAleatoire.Next(0, 40) == 0)
            {
                Image m = new Image();
                m.Source = new BitmapImage(new Uri("pack://application:,,,/image/meteorite.png"));

                m.Width = 50; m.Height = 50; m.Stretch = Stretch.Fill;

                // Position x aléatoire
                Canvas.SetLeft(m, generateurAleatoire.Next(200, 800));
                Canvas.SetTop(m, -60);

                canvasJeu.Children.Add(m);
                meteorites.Add(m);
            }

            // mouvement
            for (int i = meteorites.Count - 1; i >= 0; i--)
            {
                Image m = meteorites[i];

                // chute vers le bas
                Canvas.SetTop(m, Canvas.GetTop(m) + vitesseMeteorite);

                // effet de parallaxe 
                if (goRight) Canvas.SetLeft(m, Canvas.GetLeft(m) - vitesseFond);
                else if (goLeft) Canvas.SetLeft(m, Canvas.GetLeft(m) + vitesseFond);

                // suppression si sortie de l'écran
                if (Canvas.GetTop(m) > 500)
                {
                    canvasJeu.Children.Remove(m);
                    meteorites.RemoveAt(i);
                }
            }
        }

        private void VerifierCollisions()
        {
            // hitbox joueur
            Rect rPerso = new Rect(Canvas.GetLeft(imgPerso), Canvas.GetTop(imgPerso), imgPerso.Width, imgPerso.Height);

            // collision lave
            Rect rLave = new Rect(Canvas.GetLeft(img_lave) + 20, Canvas.GetTop(img_lave), img_lave.Width - 20, img_lave.Height);
            if (rPerso.IntersectsWith(rLave))
            {
                GameOver("VOUS AVEZ ETE AVALER PAR LA LAVE !");
                return;
            }

            // collision meteorites
            foreach (Image m in meteorites)
            {
                Rect rMeteor = new Rect(Canvas.GetLeft(m) + 10, Canvas.GetTop(m) + 10, m.Width - 20, m.Height - 20);
                if (rPerso.IntersectsWith(rMeteor))
                {
                    GameOver("UNE METEORITE VOUS A ECRASER !");
                    return;
                }
            }
        }

        private void GameOver(string message)
        {
            estGameOver = true;
            minuterie.Stop();

            //afficher UCPerdus
            overlayPerdu.SetInfos(message, lblTemps.Content.ToString());
            overlayPerdu.Visibility = Visibility.Visible;
        }


        private void ResetPartie()
        {
            // enlever meteorite
            foreach (Image m in meteorites) canvasJeu.Children.Remove(m);
            meteorites.Clear();

            // Reset Positions
            Canvas.SetLeft(imgfond1, 0);
            Canvas.SetLeft(imgfond2, 800);
            Canvas.SetLeft(img_lave, -250);
            Canvas.SetTop(imgPerso, sol);

            // reset tout
            tempsJeu = TimeSpan.Zero;
            lblTemps.Content = "0 s";
            estGameOver = false;
            estEnSaut = false;
            force = 0;
            goRight = false;
            goLeft = false;

            // cache l'ecran perdu
            overlayPerdu.Visibility = Visibility.Collapsed;
            Window window = Window.GetWindow(this);
            if (window != null) window.Focus();

            minuterie.Start();
        }

        //LOGIQUE DECOR ET ANIMATION 

        private void BougerFond(int vitesse)
        {
            // déplace les deux images de fond
            Canvas.SetLeft(imgfond1, Canvas.GetLeft(imgfond1) + vitesse);
            Canvas.SetLeft(imgfond2, Canvas.GetLeft(imgfond2) + vitesse);

            if (vitesse < 0) // On avance
            {
                if (Canvas.GetLeft(imgfond1) <= -LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond1, Canvas.GetLeft(imgfond2) + LARGEUR_IMAGE);
                if (Canvas.GetLeft(imgfond2) <= -LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond2, Canvas.GetLeft(imgfond1) + LARGEUR_IMAGE);
            }
            else if (vitesse > 0) // on recule
            {
                if (Canvas.GetLeft(imgfond1) >= LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond1, Canvas.GetLeft(imgfond2) - LARGEUR_IMAGE);
                if (Canvas.GetLeft(imgfond2) >= LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond2, Canvas.GetLeft(imgfond1) - LARGEUR_IMAGE);
            }
        }

        private void AnimerPerso()
        {
            nbAnimation++;
            if (nbAnimation % 4 == 0)
            {
                int index = (nbAnimation / 4) % persos.Length;
                if (persos.Length > 0) imgPerso.Source = persos[index];
            }
        }

        // INITIALISATION

        private void InitializeImages()
        {
            string prefix = (nomPersonnage == "Lina") ? "Lina_" : "Johncourt_";

            for (int i = 0; i < persos.Length; i++)
            {
                // charge les images Johncourt_1 à 6 ou Lina_1 à 6
                persos[i] = new BitmapImage(new Uri($"pack://application:,,,/image/{prefix}{i + 1}.png"));
            }
            if (persos.Length > 0 && persos[0] != null) imgPerso.Source = persos[0];
        }

        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }

        //externe

        public void MettreEnPause()
        {
            if (minuterie.IsEnabled) minuterie.Stop();
        }

        public void Reprendre()
        {
            if (!minuterie.IsEnabled && !estGameOver) minuterie.Start();
        }


        private void InitSon()
        {
            win = new SoundPlayer(Application.GetResourceStream(

           new Uri("/Sons/WinSound.wav", UriKind.Relative)).Stream);

        }

    }
}