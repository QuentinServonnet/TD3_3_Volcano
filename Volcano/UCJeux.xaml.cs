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
using System.Windows.Threading;

namespace Volcano
{
    /// <summary>
    /// Interaction logic for UCJeux.xaml
    /// </summary>
    public partial class UCJeux : UserControl
    {
        private BitmapImage[] persos = new BitmapImage[8];
        private DispatcherTimer minuterie;
        private int nb = 0;
        public UCJeux()
        {
            InitializeComponent();
            InitializeImages();
            InitializeTimer();
        }
        private void InitializeImages()
        {
            for (int i = 0; i < 6; i++)
                persos[i] = new BitmapImage(new Uri($"pack://application:,,,/image/Johncourt_{i + 1}.png"));
        }
        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            // configure l'intervalle du Timer :62 images par s
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            // associe l’appel de la méthode Jeu à la fin de la minuterie
            minuterie.Tick += Jeu;
            // lancement du timer
            minuterie.Start();

        }
        private void Jeu(object sender, EventArgs e)
        {
            Deplace(imgfond1, 2);
            Deplace(imgfond2, 2);


            nb++;
            if (nb == persos.Length * 4)
                nb = 0;
            if (nb % 4 == 0)
                imgPerso.Source = persos[nb / 4];
        }
        public void Deplace(Image image, int pas)
        {
            Canvas.SetLeft(image, Canvas.GetLeft(image) - pas);

            if (Canvas.GetLeft(image) + image.Width <= 0)
                Canvas.SetLeft(image, canvasJeu.ActualWidth);
        }

     
    }
}
