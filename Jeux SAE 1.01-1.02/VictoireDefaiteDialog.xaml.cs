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
    /// Logique d'interaction pour VictoireDefaiteDialog.xaml
    /// </summary>
    public partial class VictoireDefaiteDialog : Window
    {
        public VictoireDefaiteDialog(string message)
        {
            InitializeComponent();
            messageText.Text = message;
        }

        private void BtnRejouer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnRetourMenu_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
