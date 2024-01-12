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
        private NiveauDifficulte difficulteActuelle;
        public MainWindow()
        {

            MenuJouer fenetreMenu = new();
            fenetreMenu.ShowDialog();

            if (fenetreMenu.DialogResult == false)
                Application.Current.Shutdown();
            
            
            ChoixNiveau fenetreChoix = new ChoixNiveau();
            fenetreChoix.ShowDialog();

            if (fenetreChoix.DialogResult == true)
            {
                difficulteActuelle = fenetreChoix.DifficulteChoisie;
                ExecuterNiveau();
            }
        }

        private void ExecuterNiveau()
        {
            switch (difficulteActuelle)
            {
                case NiveauDifficulte.Apprenti:
                    ExecuterNiveauApprenti();
                    break;
                case NiveauDifficulte.Amateur:
                    ExecuterNiveauAmateur();
                    break;
                case NiveauDifficulte.Pro:
                    ExecuterNiveauPro();
                    break;
                default:
                    break;
            }
        }

        private void ExecuterNiveauApprenti()
        {
            //Niveau Apprenti (Naruto)

        }

        private void ExecuterNiveauAmateur()
        {
            //Niveau Amateur (One Piece)
        }

        private void ExecuterNiveauPro()
        {
            //Niveau Pro (DBZ)
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
           //Déplacements Personnage
        }
    }
}
