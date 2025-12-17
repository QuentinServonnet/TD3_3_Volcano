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
            _player = new MediaPlayer();

            _player.Open(new Uri("Assets/musique.mp3", UriKind.Relative));

            // volume initial (0.0 = muet, 1.0 = maximum)
            _player.Volume = 0.5;

            cbDifficulte.SelectionChanged += CbDifficulte_SelectionChanged;

            // lecture en boucle
            _player.MediaEnded += (s, e) =>
                {
                _player.Position = TimeSpan.Zero;
                _player.Play();
            };

                _player.Play();
        }

        private void CbDifficulte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDifficulte.SelectedItem is ComboBoxItem item)
            {
                ((MainWindow)Application.Current.MainWindow).NiveauDifficulte = item.Content.ToString();
            }
        }

        // changer le volume depuis un Slider
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

