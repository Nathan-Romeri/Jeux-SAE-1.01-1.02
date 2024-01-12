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
    /// Logique d'interaction pour ChoixNiveau.xaml
    /// </summary>
    public partial class ChoixNiveau : Window
    {
        public NiveauDifficulte DifficulteChoisie { get; private set; }

        public ChoixNiveau()
        {
            InitializeComponent();

        }

        private void Apprenti_Click(object sender, RoutedEventArgs e)
        {
            DifficulteChoisie = NiveauDifficulte.Apprenti;
            this.DialogResult = true;
        }

        private void Amateur_Click(object sender, RoutedEventArgs e)
        {
            DifficulteChoisie = NiveauDifficulte.Amateur;
            this.DialogResult = true;
        }

        private void Pro_Click(object sender, RoutedEventArgs e)
        {
            DifficulteChoisie = NiveauDifficulte.Pro;
            this.DialogResult = true;
        }
    }
    public enum NiveauDifficulte
    {
        Apprenti,
        Amateur,
        Pro
    }
}
