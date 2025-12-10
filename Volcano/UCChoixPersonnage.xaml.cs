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
        private void BtnLisa_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("test Lisa");
            // suite jeux
        }

        private void BtnJohn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("test john");
            // suite jeux
        }
    }
}
