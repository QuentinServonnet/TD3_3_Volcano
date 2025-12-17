using System;
using System.Collections.Generic;
using System.Printing;
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
        private BitmapImage textureMeteorite;
        private BitmapImage textureObstacle;

        private DispatcherTimer minuterie;
        private int nbAnimation = 0;
        private string nomPersonnage;

        // etat du jeu
        private TimeSpan tempsJeu;
        private DateTime dernierTick;
        private bool estGameOver = false;

        // mouvements et environnement
        private bool goRight = false;
        private bool goLeft = false;
        private int vitesseFond = 6; // Sera modifié par la difficulté
        private const double LARGEUR_IMAGE = 800;

        // lave
        private double vitesseLave = 1.5; // Sera modifié par la difficulté

        //  Météorites
        private List<Image> meteorites = new List<Image>();
        private Random generateurAleatoire = new Random();
        private int vitesseMeteorite = 10;
        private int frequenceMeteorite = 40; // Plus c'est bas, plus il y en a

        private List<Image> obstacles = new List<Image>();
        private int tempsAttenteObstacle = 0;
        private int frequenceObstacle = 60; // Plus c'est bas, plus il y en a

        // physique
        private bool estEnSaut = false;
        private int force = 0;
        private int gravite = 1;
        private int puissanceSaut = 16;
        private double sol;

        public UCJeux(string personnageChoisi = "John", string niveauDifficulte = "Normal")
        {
            InitializeComponent();
            this.nomPersonnage = personnageChoisi; //on sauvegarde le choix

            // 1. On applique la difficulté AVANT de lancer le jeu
            ConfigurerDifficulte(niveauDifficulte);

            InitializeImages();// charge les images du perso
            InitializeTimer();  // lance la boucle de jeu

            tempsJeu = TimeSpan.Zero;// reset du chrono
            lblTemps.Content = "00:00";

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

        // difficulte
        private void ConfigurerDifficulte(string niveau)
        {
            switch (niveau)
            {
                case "Facile":
                    vitesseFond = 5;
                    vitesseLave = 1.0;
                    frequenceMeteorite = 60;
                    frequenceObstacle = 80;
                    break;

                case "Normal":
                    vitesseFond = 7;
                    vitesseLave = 1.5;
                    frequenceMeteorite = 40;
                    frequenceObstacle = 60;
                    break;

                case "Difficile":
                    vitesseFond = 9;
                    vitesseLave = 2.0;
                    frequenceMeteorite = 25;
                    frequenceObstacle = 45;
                    break;

                case "Enfer":
                    vitesseFond = 12;
                    vitesseLave = 6;
                    frequenceMeteorite = 15;
                    frequenceObstacle = 30;
                    break;
            }
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
                if (persos.Length > 0 && persos[0] != null) imgPerso.Source = persos[0];
            }
        }

        // BOUCLE DE JEU

        private void Jeu(object sender, EventArgs e)
        {
            if (estGameOver) return;
            // MISE A JOUR DU CHRONO
            DateTime maintenant = DateTime.Now;
            TimeSpan ecart = maintenant - dernierTick;
            tempsJeu += ecart;
            dernierTick = maintenant;

            lblTemps.Content = tempsJeu.ToString(@"mm\:ss");

            //PHYSIQUE
            if (estEnSaut || Canvas.GetTop(imgPerso) < sol)
            {
                Canvas.SetTop(imgPerso, Canvas.GetTop(imgPerso) + force);
                force += gravite;// la gravité tire vers le bas

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
            GererObstacles();
            VerifierCollisions();// vérifie si on est mort

            if (goRight)
            {
                imgPerso.RenderTransform = new ScaleTransform(1, 1);
                BougerFond(-vitesseFond);
                AnimerPerso();
            }
            else if (goLeft)
            {
                imgPerso.RenderTransform = new ScaleTransform(-1, 1);
                BougerFond(vitesseFond);
                AnimerPerso();
            }
        }

        // ENNEMIS

        private void BougerLave()
        {
            double posLave = Canvas.GetLeft(img_lave);
            double deplacement = vitesseLave; // la lave avance toujours un peu

            if (goRight) deplacement -= vitesseFond;
            else if (goLeft) deplacement += vitesseFond;

            Canvas.SetLeft(img_lave, posLave + deplacement);
        }

        private void GererMeteorites()
        {
            // Utilisation de la variable 'frequenceMeteorite' définie par la difficulté
            if (generateurAleatoire.Next(0, frequenceMeteorite) == 0)
            {
                if (textureMeteorite != null)
                {
                    Image m = new Image();
                    m.Source = textureMeteorite;
                    m.Width = 50; m.Height = 50; m.Stretch = Stretch.Fill;
                    // Position x aléatoire
                    Canvas.SetLeft(m, generateurAleatoire.Next(200, 800));
                    Canvas.SetTop(m, -60);

                    canvasJeu.Children.Add(m);
                    meteorites.Add(m);
                }
            }

            for (int i = meteorites.Count - 1; i >= 0; i--)
            {
                Image m = meteorites[i];
                Canvas.SetTop(m, Canvas.GetTop(m) + vitesseMeteorite);

                if (goRight) Canvas.SetLeft(m, Canvas.GetLeft(m) - vitesseFond);
                else if (goLeft) Canvas.SetLeft(m, Canvas.GetLeft(m) + vitesseFond);

                if (Canvas.GetTop(m) > 500)
                {
                    canvasJeu.Children.Remove(m);
                    meteorites.RemoveAt(i);
                }
            }
        }

        private void GererObstacles()
        {
            if (tempsAttenteObstacle > 0) tempsAttenteObstacle--;

            if (tempsAttenteObstacle <= 0 && generateurAleatoire.Next(0, frequenceObstacle) == 0)
            {
                if (textureObstacle != null)
                {
                    Image obs = new Image();
                    obs.Source = textureObstacle;
                    obs.Width = 50; obs.Height = 50; obs.Stretch = Stretch.Fill;

                    Canvas.SetLeft(obs, 850);
                    Canvas.SetTop(obs, sol + (107 - 50));

                    canvasJeu.Children.Add(obs);
                    obstacles.Add(obs);

                    tempsAttenteObstacle = 70;
                }
            }

            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                Image obs = obstacles[i];

                if (goRight) Canvas.SetLeft(obs, Canvas.GetLeft(obs) - vitesseFond);
                else if (goLeft) Canvas.SetLeft(obs, Canvas.GetLeft(obs) + vitesseFond);

                if (Canvas.GetLeft(obs) < -100 || Canvas.GetLeft(obs) > 1000)
                {
                    canvasJeu.Children.Remove(obs);
                    obstacles.RemoveAt(i);
                }
            }
        }

        //collisions

        private void VerifierCollisions()
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(imgPerso), Canvas.GetTop(imgPerso), imgPerso.Width - 15, imgPerso.Height);
            Rect lavaHitBox = new Rect(Canvas.GetLeft(img_lave) + 20, Canvas.GetTop(img_lave), img_lave.Width - 20, img_lave.Height);

            if (playerHitBox.IntersectsWith(lavaHitBox))
            {
                // collision lave
                GameOver("VOUS AVEZ ETE AVALER PAR LA LAVE !");
                return;
            }

            foreach (Image m in meteorites)
            {
                // collision meteorite
                Rect meteorHitBox = new Rect(Canvas.GetLeft(m), Canvas.GetTop(m), m.Width, m.Height);
                if (playerHitBox.IntersectsWith(meteorHitBox))
                {
                    GameOver("UNE METEORITE VOUS A ECRASE !");
                    return;
                }
            }

            foreach (Image obs in obstacles)
            {
                //collision pieges
                Rect obstacleHitBox = new Rect(Canvas.GetLeft(obs), Canvas.GetTop(obs), obs.Width, obs.Height);
                if (playerHitBox.IntersectsWith(obstacleHitBox))
                {
                    GameOver("VOUS AVEZ HEURTE UN OBSTACLE !");
                    return;
                }
            }
        }

        //LOGIQUE DECOR ET ANIMATION 

        private void BougerFond(int vitesse)
        {// déplace les deux images de fond
            Canvas.SetLeft(imgfond1, Canvas.GetLeft(imgfond1) + vitesse);
            Canvas.SetLeft(imgfond2, Canvas.GetLeft(imgfond2) + vitesse);

            if (vitesse < 0)//avance
            {
                if (Canvas.GetLeft(imgfond1) <= -LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond1, Canvas.GetLeft(imgfond2) + LARGEUR_IMAGE);
                if (Canvas.GetLeft(imgfond2) <= -LARGEUR_IMAGE)
                    Canvas.SetLeft(imgfond2, Canvas.GetLeft(imgfond1) + LARGEUR_IMAGE);
            }
            else if (vitesse > 0)//recule
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
                if (persos.Length > 0 && persos[index] != null) imgPerso.Source = persos[index];
            }
        }

        private void GameOver(string message)
        {
            estGameOver = true;
            minuterie.Stop();
            overlayPerdu.SetInfos(message, lblTemps.Content.ToString());
            overlayPerdu.Visibility = Visibility.Visible;
        }

        private void ResetPartie()
        {
            foreach (Image m in meteorites) canvasJeu.Children.Remove(m);
            meteorites.Clear();

            foreach (Image obs in obstacles) canvasJeu.Children.Remove(obs);
            obstacles.Clear();
            tempsAttenteObstacle = 0;

            Canvas.SetLeft(imgfond1, 0);
            Canvas.SetLeft(imgfond2, 800);
            Canvas.SetLeft(img_lave, -537);
            Canvas.SetTop(imgPerso, sol);

            tempsJeu = TimeSpan.Zero;
            lblTemps.Content = "00:00";
            estGameOver = false;
            estEnSaut = false;
            force = 0;
            goRight = false;
            goLeft = false;

            overlayPerdu.Visibility = Visibility.Collapsed;
            Window window = Window.GetWindow(this);
            if (window != null) window.Focus();

            dernierTick = DateTime.Now;
            minuterie.Start();
        }
        // INITIALISATION
        private void InitializeImages()
        {
                textureMeteorite = new BitmapImage();
                textureMeteorite.BeginInit();
                textureMeteorite.UriSource = new Uri("pack://application:,,,/image/meteorite.png");
                textureMeteorite.CacheOption = BitmapCacheOption.OnLoad;
                textureMeteorite.EndInit();
                textureMeteorite.Freeze();

                textureObstacle = new BitmapImage();
                textureObstacle.BeginInit();
                textureObstacle.UriSource = new Uri("pack://application:,,,/image/obstacle.jpg");
                textureObstacle.CacheOption = BitmapCacheOption.OnLoad;
                textureObstacle.EndInit();
                textureObstacle.Freeze();

            string prefix = (nomPersonnage == "Lina") ? "Lina_" : "Johncourt_";
            for (int i = 0; i < persos.Length; i++)
            {
                    persos[i] = new BitmapImage();
                    persos[i].BeginInit();
                    persos[i].UriSource = new Uri($"pack://application:,,,/image/{prefix}{i + 1}.png");
                    persos[i].CacheOption = BitmapCacheOption.OnLoad;
                    persos[i].EndInit();
                    persos[i].Freeze();
                {
                    if (nomPersonnage == "Lina")
                    {
                        persos[i] = new BitmapImage(new Uri("pack://application:,,,/image/Lina.png"));
                    }
                }
            }

            if (persos.Length > 0 && persos[0] != null) imgPerso.Source = persos[0];
        }

        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Tick += Jeu;
            dernierTick = DateTime.Now;
            minuterie.Start();
        }

        public void MettreEnPause()
        {
            if (minuterie.IsEnabled) minuterie.Stop();
        }

        public void Reprendre()
        {
            if (!minuterie.IsEnabled && !estGameOver)
            {
                dernierTick = DateTime.Now;
                minuterie.Start();
            }
        }
    }
}