using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Jeux_SAE_1._01_1._02
{
    public partial class MainWindow : Window
    {
        private NiveauDifficulte difficulteActuelle;
        private Canvas canvas;
        private Rectangle personnage;
        private int objetsCollectes;
        private DispatcherTimer timer;
        private Random random;
        private TextBlock tempsTextBlock;
        private TextBlock objetsTextBlock;
        private DateTime tempsLimite;
        private DateTime tempsEcoule;
        private DispatcherTimer spawnTimer;
        private Rectangle ennemi;
        private Rectangle projectileEnnemi;



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

                // Initialisation :
                //Canvas
                canvas = new Canvas();
                this.Content = canvas;

                //Nombres aléatoires
                random = new Random();

                // Ajout de l'image en tant que fond du Canvas
                string cheminImage = ObtenirCheminImageDuNiveau(difficulteActuelle);
                ImageBrush fondImage = new ImageBrush(new BitmapImage(new Uri(cheminImage, UriKind.RelativeOrAbsolute)));
                canvas.Background = fondImage;

                // Création du personnage
                personnage = new Rectangle();
                personnage.Width = 150;
                personnage.Height = 150;
                personnage.Fill = Brushes.Red;

                Canvas.SetLeft(personnage, 0);
                Canvas.SetTop(personnage, 0);

                // Ajout du personnage au Canvas (après l'ajout de l'image de fond)
                canvas.Children.Add(personnage);

                // Initialisation du compteur d'objets collectés
                objetsCollectes = 0;

                // Configuration du timer pour suivre le temps de jeu
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();

                // Configuration du timer pour le spawn aléatoire d'objets
                spawnTimer = new DispatcherTimer();
                spawnTimer.Interval = TimeSpan.FromSeconds(5); // Réglez le délai de spawn selon vos besoins
                spawnTimer.Tick += SpawnTimer_Tick;
                spawnTimer.Start();

                // Ajout du TextBlock pour le compteur de temps
                tempsTextBlock = new TextBlock();
                tempsTextBlock.FontSize = 16;
                tempsTextBlock.Foreground = Brushes.White;
                tempsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                Canvas.SetTop(tempsTextBlock, 10);
                canvas.Children.Add(tempsTextBlock);

                // Ajout du TextBlock pour le compteur d'objets
                objetsTextBlock = new TextBlock();
                objetsTextBlock.FontSize = 16;
                objetsTextBlock.Foreground = Brushes.White;
                objetsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                Canvas.SetTop(objetsTextBlock, 30);
                canvas.Children.Add(objetsTextBlock);

                //Ajout ennemie





                ennemi = new Rectangle
                {
                    Width = 80,
                    Height = 80,
                    Fill = Brushes.Yellow
                };

                Canvas.SetRight(ennemi, canvas.ActualWidth / 20 - ennemi.Width / 2 + 40);
                Canvas.SetTop(ennemi, 20);

                canvas.Children.Add(ennemi);

                projectileEnnemi = new Rectangle
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red
                };

                ExecuterNiveau();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan tempsRestant = tempsLimite - tempsEcoule;
            tempsTextBlock.Text = $"Temps restant : {tempsRestant.Minutes:00}:{tempsRestant.Seconds:00}";

            if (tempsEcoule >= tempsLimite)
            {
                // Le temps est écoulé, terminez le jeu ou passez au niveau suivant
                timer.Stop();
                // Ajoutez ici la logique pour passer au niveau suivant ou afficher le menu de choix de niveau
            }
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            List<UIElement> elementsToRemove = new List<UIElement>(canvas.Children.OfType<UIElement>());

            foreach (UIElement element in elementsToRemove)
            {
                if (element is Image objet)
                {
                    double left1 = Canvas.GetLeft(objet);
                    double top1 = Canvas.GetTop(objet);
                    double width1 = objet.ActualWidth;
                    double height1 = objet.ActualHeight;

                    double left2 = Canvas.GetLeft(personnage);
                    double top2 = Canvas.GetTop(personnage);
                    double width2 = personnage.ActualWidth;
                    double height2 = personnage.ActualHeight;

                    if (IsCollision(left1, top1, width1, height1, left2, top2, width2, height2))
                    {
                        objetsCollectes++;

                        if (objetsCollectes >= 10)
                        {
                            timer.Stop();
                            // Ajoutez ici la logique pour passer au niveau suivant ou afficher le menu de choix de niveau
                        }

                        objetsTextBlock.Text = $"Objets : {objetsCollectes}/20";

                        // Supprimez l'ancien objet collecté du Canvas
                        canvas.Children.Remove(objet);

                        // Créez un nouvel objet GameObject avec une image
                        GameObject gameObject = new GameObject
                        {
                            Image = new Image
                            {
                                Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\item_collect3.png")),
                                Width = 50, // Ajustez la largeur de l'image
                                Height = 50, // Ajustez la hauteur de l'image
                            }
                        };

                        // Ajoutez cet objet à la liste des enfants du Canvas
                        canvas.Children.Add(gameObject.Image);

                        // Mise à jour de la position de l'objet collecté (vous pouvez ajuster cela selon vos besoins)
                        Canvas.SetLeft(gameObject.Image, random.Next(0, (int)canvas.ActualWidth));
                        Canvas.SetTop(gameObject.Image, random.Next(0, (int)canvas.ActualHeight));
                    }
                }
            }

            if (objetsCollectes >= 20)
            {
                timer.Stop();
                // Ajoutez ici la logique pour passer au niveau suivant ou afficher le menu de choix de niveau
            }
        }

        private bool IsCollision(double left1, double top1, double width1, double height1,
                           double left2, double top2, double width2, double height2)
        {
            // Vérifiez s'il y a une non-collision sur l'un des côtés
            if ((left1 + width1) < left2 || left1 > (left2 + width2) ||
                (top1 + height1) < top2 || top1 > (top2 + height2))
            {
                // Pas de collision
                return false;
            }
            else
            {
                // Collision
                return true;
            }
        }


        public class GameObject
        {
            public Image Image { get; set; }
            public DispatcherTimer Timer { get; set; }
        }
        private void ExecuterNiveau()
        {
            switch (difficulteActuelle)
            {
                case NiveauDifficulte.Apprenti:
                    personnage.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\naruto_perso_pixel.png", UriKind.RelativeOrAbsolute)));
                    break;
                case NiveauDifficulte.Amateur:
                    personnage.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\luffy_perso_pixel.png", UriKind.RelativeOrAbsolute)));
                    break;
                case NiveauDifficulte.Pro:
                    personnage.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\goku_perso_pixel.png", UriKind.RelativeOrAbsolute)));
                    break;
                default:
                    break;
            }
            AjouterImageDuNiveau(difficulteActuelle);
        }

        private void AjouterImageDuNiveau(NiveauDifficulte niveau)
        {
            Image imageFond = new Image();
            string cheminImage = ObtenirCheminImageDuNiveau(niveau);

            // Chargement de l'image en fonction du niveau
            imageFond.Source = new BitmapImage(new Uri(cheminImage, UriKind.RelativeOrAbsolute));

            // Positionnement de l'image
            Canvas.SetLeft(imageFond, 0);
            Canvas.SetTop(imageFond, 0);

            imageFond.Opacity = 0.5;

            // Ajout de l'image au Canvas
            canvas.Children.Add(imageFond);
        }

        private string ObtenirCheminImageDuNiveau(NiveauDifficulte niveau)
        {
            // Logique pour déterminer le chemin de l'image en fonction du niveau
            switch (niveau)
            {
                case NiveauDifficulte.Apprenti:
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\fond_vert.jpg";
                case NiveauDifficulte.Amateur:
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\fond_beige.jpg";
                case NiveauDifficulte.Pro:
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\fond_bleu.jpg";
                default:
                    // Cas par défaut, retourne une image par défaut
                    return "Images/Default.jpg";
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            double deplacement = 30;

            switch (e.Key)
            {
                case Key.Up:
                    if (Canvas.GetTop(personnage) - deplacement >= 0)
                        Canvas.SetTop(personnage, Canvas.GetTop(personnage) - deplacement);
                    break;
                case Key.Down:
                    if (Canvas.GetTop(personnage) + personnage.ActualHeight + deplacement <= canvas.ActualHeight)
                        Canvas.SetTop(personnage, Canvas.GetTop(personnage) + deplacement);
                    break;
                case Key.Left:
                    if (Canvas.GetLeft(personnage) - deplacement >= 0)
                        Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - deplacement);
                    break;
                case Key.Right:
                    if (Canvas.GetLeft(personnage) + personnage.ActualWidth + deplacement <= canvas.ActualWidth)
                        Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + deplacement);
                    break;
                default:
                    break;
            }
        }
        private void AfficherMessageVictoireOuDefaite(string message)
        {
            VictoireDefaiteDialog dialog = new VictoireDefaiteDialog(message);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                // L'utilisateur a choisi de rejouer
                RejouerNiveau();
            }
            else
            {
                // L'utilisateur a choisi de retourner au menu
                RetourMenu();
            }
        }

        private void RejouerNiveau()
        {
            // Ajoutez ici la logique pour réinitialiser le niveau
            // (réinitialisation des objets, du personnage, du compteur d'objets, etc.)
        }

        private void RetourMenu()
        {
            // Ajoutez ici la logique pour revenir au menu de choix de niveau
            // (par exemple, montrer la boîte de dialogue de choix de niveau)
            AfficherChoixNiveau();
        }

        private void AfficherChoixNiveau()
        {
            ChoixNiveau fenetreChoix = new ChoixNiveau();
            fenetreChoix.ShowDialog();

            if (fenetreChoix.DialogResult == true)
            {
                difficulteActuelle = fenetreChoix.DifficulteChoisie;
                ExecuterNiveau(); // ou toute autre logique que vous souhaitez exécuter après avoir choisi le niveau
            }
            else
            {
                // L'utilisateur a annulé la sélection du niveau, ajoutez la logique appropriée ici
            }
        }
        private void VerifierVictoire()
        {
            if (objetsCollectes >= 10)
            {
                timer.Stop();

            }
            else
            {
                // Vérifier si le temps imparti est écoulé
                if (tempsEcoule >= tempsLimite)
                {
                    timer.Stop();

                }
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            // ... (le code existant)

            // Logique pour faire tirer le projectile par l'ennemi
            if (random.Next(100) < 5) // Une chance de 5% de tirer à chaque tick (ajustez selon vos besoins)
            {
                ShootEnemyProjectile();
            }
        }

        private void ShootEnemyProjectile()
        {
            Rectangle newEnemyProjectile = new Rectangle
            {
                Width = projectileEnnemi.Width,
                Height = projectileEnnemi.Height,
                Fill = projectileEnnemi.Fill.Clone()
            };

            Canvas.SetLeft(newEnemyProjectile, Canvas.GetLeft(ennemi) + ennemi.Width / 2 - newEnemyProjectile.Width / 2);
            Canvas.SetTop(newEnemyProjectile, Canvas.GetTop(ennemi) + ennemi.Height);

            canvas.Children.Add(newEnemyProjectile);

            // Animation du projectile (par exemple, déplacement vers le bas)
            DoubleAnimation animation = new DoubleAnimation
            {
                From = Canvas.GetTop(newEnemyProjectile),
                To = canvas.ActualHeight,
                Duration = TimeSpan.FromSeconds(2) // Ajustez la durée du déplacement
            };

            newEnemyProjectile.BeginAnimation(Canvas.TopProperty, animation);
        }

















































    }

}
