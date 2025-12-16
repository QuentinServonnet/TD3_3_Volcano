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
    /// Interaction logic for UCParametres.xaml
    /// </summary>
    public partial class UCParametres : UserControl
    {
        private MediaPlayer _player;

        public UCParametres()
        {
            InitializeComponent();
            // Initialisation du lecteur 
            _player = new MediaPlayer();

            try
            {
                // Charger un fichier audio (WAV, MP3, etc.)
                _player.Open(new Uri("Assets/musique.mp3", UriKind.Relative));

                // Volume initial (0.0 = muet, 1.0 = maximum)t
                _player.Volume = 0.5;

                // Lecture en boucle
                _player.MediaEnded += (s, e) =>
                {
                    _player.Position = TimeSpan.Zero;
                    _player.Play();
                };

                _player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du son : " + ex.Message);
            }
        }

        // Exemple : changer le volume depuis un Slider
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null)
            {
                // Le slider doit avoir une plage de 0.0 à 1.0
                _player.Volume = e.NewValue;
            }
        }

    }
}

