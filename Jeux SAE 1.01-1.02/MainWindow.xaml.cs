using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            MenuJouer fenetreMenu = new MenuJouer();
            fenetreMenu.ShowDialog();

            if (fenetreMenu.DialogResult == false)

                Application.Current.Shutdown();
        }



        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Déplacez le personnage
            switch (e.Key)
            {
                case Key.Up:
                    personnage.Margin = new Thickness(personnage.Margin.Left, personnage.Margin.Top - 20, 0, 0);
                    break;
                case Key.Down:
                    personnage.Margin = new Thickness(personnage.Margin.Left, personnage.Margin.Top + 20, 0, 0);
                    break;
                case Key.Left:
                    personnage.Margin = new Thickness(personnage.Margin.Left - 20, personnage.Margin.Top, 0, 0);
                    break;
                case Key.Right:
                    personnage.Margin = new Thickness(personnage.Margin.Left + 20, personnage.Margin.Top, 0, 0);
                    break;
            }
        }
    }
}
