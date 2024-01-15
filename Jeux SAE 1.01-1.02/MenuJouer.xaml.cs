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

namespace Jeux_SAE_1._01_1._02
{
   
    public partial class MenuJouer : Window
    {
        public MenuJouer()
        {
            InitializeComponent();

        }

        private void boutonJouer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close(); // Fermer la fenêtre du menu de jeu
        }


        private void boutonQuitter_Click(object sender, RoutedEventArgs e)
        {
            // Fermer toutes les fenêtres
            Application.Current.Shutdown();
        }

        private void boutonParametres_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre des paramètres
            Paramètres fenetreParametres = new Paramètres();
            fenetreParametres.ShowDialog();
        }


    }
}
