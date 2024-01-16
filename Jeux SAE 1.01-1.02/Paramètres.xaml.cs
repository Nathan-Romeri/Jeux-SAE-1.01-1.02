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
using System.Windows.Shapes;

namespace Jeux_SAE_1._01_1._02
{
    /// <summary>
    /// Logique d'interaction pour Paramètres.xaml
    /// </summary>
    public partial class Paramètres : Window
    {
        public Paramètres()
        {
            InitializeComponent();
        }

        private void boutonFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Fermer la fenêtre des paramètres
        }
    }
}
