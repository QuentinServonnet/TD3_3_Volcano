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
        }

        private void but_jouer_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new UCChoixPersonnage();
        }

        private void but_parametre_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new UCParametres();
        }
    }
}