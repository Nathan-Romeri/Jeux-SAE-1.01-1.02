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
            InitializeComponent();
            MenuJouer fenetreMenu = new MenuJouer();
            fenetreMenu.ShowDialog();



            if (fenetreMenu.DialogResult == false)
                Application.Current.Shutdown();
            InitializeComponent();

           

        }



        private void MainWindow__KeyDown(object sender, KeyEventArgs e)
        {
            // Déplacez le personnage en fonction de la touche appuyée
            switch (e.Key)
            {
                case Key.Up:
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) - 10);
                    break;
                case Key.Down:
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) + 10);
                    break;
                case Key.Left:
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - 10);
                    break;
                case Key.Right:
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + 10);
                    break;
            }
        }

        private void Rectangle_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
