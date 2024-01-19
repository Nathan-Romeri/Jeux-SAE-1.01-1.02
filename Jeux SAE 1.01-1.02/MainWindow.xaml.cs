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
        private TextBlock messageTextBlock;
        private TextBlock vieTextBlock;
        private List<Projectile> projectiles;
        private Random random2;
        private int maxProjectiles = 1;
        private ChoixNiveau fenetreChoix;
        private Button pauseButton;


        private bool jeuEnPause = false;
        private DateTime tempsEnPause;

        private int viesRestantes;

        public MainWindow()
        {
            MenuJouer fenetreMenu = new();
            fenetreMenu.ShowDialog();

            if (fenetreMenu.DialogResult == false)
                Application.Current.Shutdown();

            fenetreChoix = new ChoixNiveau();
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

                //Projectiles aléatoire
                random2 = new Random();

                // Initialisation de la liste des projectiles
                projectiles = new List<Projectile>();

                // Initialisez le nombre de vies
                viesRestantes = 3;

                //Instance de fentre choix
                fenetreChoix = new ChoixNiveau();

                // Ajout de l'image en tant que fond du Canvas
                string cheminImage = ObtenirCheminImageDuNiveau(difficulteActuelle);
                ImageBrush fondImage = new ImageBrush(new BitmapImage(new Uri(cheminImage, UriKind.RelativeOrAbsolute)));
                canvas.Background = fondImage;

                // Création du personnage
                personnage = new Rectangle();
                personnage.Width = 120;
                personnage.Height = 120;
                personnage.Fill = Brushes.Red;

                Canvas.SetLeft(personnage, 680);
                Canvas.SetTop(personnage, 100);

                // Ajout du personnage au Canvas (après l'ajout de l'image de fond)
                canvas.Children.Add(personnage);

                // Initialisation du compteur d'objets collectés
                objetsCollectes = 0;

                // Configuration du timer pour suivre le temps de jeu
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                // Configuration du timer pour le spawn aléatoire d'objets
                spawnTimer = new DispatcherTimer();
                spawnTimer.Interval = TimeSpan.FromSeconds(0.2); // Réglez le délai de spawn selon vos besoins
                spawnTimer.Tick += SpawnTimer_Tick;

                // Ajout du TextBlock pour le compteur de temps
                tempsTextBlock = new TextBlock();
                tempsTextBlock.FontSize = 48;
                tempsTextBlock.Foreground = Brushes.Red;
                tempsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                tempsTextBlock.FontWeight = FontWeights.Bold;

                Canvas.SetTop(tempsTextBlock, 2);
                Canvas.SetLeft(tempsTextBlock, 640);
                canvas.Children.Add(tempsTextBlock);

                // Ajout du TextBlock pour le compteur d'objets
                objetsTextBlock = new TextBlock();
                objetsTextBlock.FontSize = 30;
                objetsTextBlock.Foreground = Brushes.Blue;
                objetsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                objetsTextBlock.FontFamily = new FontFamily("Showcard Gothic");
                Canvas.SetTop(objetsTextBlock, 10);
                canvas.Children.Add(objetsTextBlock);

                // Ajout du TextBlock pour le compteur  de vies
                vieTextBlock = new TextBlock();
                vieTextBlock.FontSize = 30;
                vieTextBlock.Foreground = Brushes.Purple;
                vieTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                vieTextBlock.FontFamily = new FontFamily("Showcard Gothic");

                // Initialiser le texte du compteur de vie dès le début
                vieTextBlock.Text = $"Vie : {viesRestantes}/3";

                Canvas.SetTop(vieTextBlock, 60);
                Canvas.SetLeft(vieTextBlock, 10);
                canvas.Children.Add(vieTextBlock);

                Image pauseImage = new Image();
                pauseImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\Pause_Button_Icon.png", UriKind.RelativeOrAbsolute));
                pauseImage.Width = 100;
                pauseImage.Height = 100;

                // Gérez l'événement Click ou MouseDown de l'image
                pauseImage.Tag = "PauseButton";
                pauseImage.MouseDown += PauseImage_MouseDown; 

                // Ajoutez l'image au Canvas
                canvas.Children.Add(pauseImage);

                Canvas.SetLeft(pauseImage, 1320);
                Canvas.SetTop(pauseImage, 10);

                InitialiserNiveau();
            }
            
        }

        private void InitialiserNiveau()
        {
            ExecuterNiveau();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Démarrez le timer avec un temps initial de 2 minutes.
            tempsLimite = DateTime.Now.AddSeconds(120);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Mettez à jour le temps écoulé
            tempsEcoule = DateTime.Now;

            TimeSpan tempsRestant = tempsLimite - tempsEcoule;
            tempsTextBlock.Text = $" {tempsRestant.Minutes:00}:{tempsRestant.Seconds:00}";

            // Vérifiez si le temps imparti est écoulé
            if (tempsEcoule >= tempsLimite)
            {
                timer.Stop();
                AfficherMessageVictoireOuDefaite("Temps écoulé. Vous avez perdu !");
            }
        }

        private void SpawnTimer_Tick(object sender, EventArgs e)
        {
            List<UIElement> elementsToRemove = new List<UIElement>(canvas.Children.OfType<UIElement>());

            foreach (UIElement element in elementsToRemove)
            {
                if (element is Image objet && objet.Tag != "PauseButton")
                {
                    double left1 = Canvas.GetLeft(objet);
                    double top1 = Canvas.GetTop(objet);
                    double width1 = objet.ActualWidth;
                    double height1 = objet.ActualHeight;

                    double left2 = Canvas.GetLeft(personnage);
                    double top2 = Canvas.GetTop(personnage);
                    double width2 = personnage.ActualWidth;
                    double height2 = personnage.ActualHeight;

                    // Mettez à jour le texte du compteur de vie
                    vieTextBlock.Text = $"Vie : {viesRestantes}/3";

                    if (IsCollision(left1, top1, width1, height1, left2, top2, width2, height2))
                    {
                        objetsCollectes++;

                        if (objetsCollectes >= 3)
                        {
                            timer.Stop();
                            spawnTimer.Stop();
                            AfficherMessageVictoireOuDefaite("Vous avez gagné !");
                            return; // Sortez de la méthode après la victoire
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

                        // Mise à jour de la position de l'objet collecté 
                        double x = random.Next(0, (int)(canvas.ActualWidth - gameObject.Image.Width));
                        double y = random.Next(0, (int)(canvas.ActualHeight - gameObject.Image.Height));

                        Canvas.SetLeft(gameObject.Image, x);
                        Canvas.SetTop(gameObject.Image, y);
                    }

                   
                }
            }

            // Vérifiez si le nombre d'objets collectés atteint 20
            if (objetsCollectes >= 20)
            {
                timer.Stop();
                spawnTimer.Stop();
            }

            foreach (Projectile projectile in projectiles)
            {
                double playerLeft = Canvas.GetLeft(personnage) + personnage.ActualWidth / 2;
                double playerTop = Canvas.GetTop(personnage) + personnage.ActualHeight / 2;

                double projectileLeft = Canvas.GetLeft(projectile.GetRectangle());
                double projectileTop = Canvas.GetTop(projectile.GetRectangle());

                double distance = Math.Sqrt(Math.Pow(playerLeft - projectileLeft, 2) + Math.Pow(playerTop - projectileTop, 2));

                if (distance < 20)
                {
                    viesRestantes--;

                    if (viesRestantes == 0)
                    {
                        timer.Stop();
                        spawnTimer.Stop();
                        AfficherMessageVictoireOuDefaite("Vous avez perdu !");
                        return; // Sortez de la méthode après la défaite
                    }

                    vieTextBlock.Text = $"Vie : {viesRestantes}/3";
                }

                projectile.Move(playerLeft, playerTop, projectiles);
            }


            int currentProjectiles = projectiles.Count;

            // Vérifiez si le nombre de projectiles est inférieur à la limite
            if (currentProjectiles < maxProjectiles)
            {
                // Générez un projectile
                GenerateProjectile();
            }

            GenerateProjectile();

        }

        private bool IsCollision(double left1, double top1, double width1, double height1,
                           double left2, double top2, double width2, double height2, bool isProjectile = false)
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
                if (!isProjectile)
                {
                    // Handle collision with regular objects
                    return true;
                }
                else
                {
                    // Handle collision with projectiles (if needed)
                    // For now, let's assume projectiles don't collide with each other
                    return false;
                }
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
            // Initialisation du compteur d'objets collectés
            objetsCollectes = 0;

            // Initialisation du temps
            tempsLimite = DateTime.Now.AddMinutes(2); // 2 minutes de jeu

            // Démarrage des timers après l'initialisation
            timer.Start();
            spawnTimer.Start();

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
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\map_naruto.png";
                case NiveauDifficulte.Amateur:
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\map_luffy.png";
                case NiveauDifficulte.Pro:
                    return AppDomain.CurrentDomain.BaseDirectory + "img\\map_goku.png";
                default:
                    // Cas par défaut, retourne une image par défaut
                    return "Images/Default.jpg";
            }
        }

        private void PauseImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!jeuEnPause)
            {
                // Mettez le jeu en pause
                timer.Stop();
                jeuEnPause = true;

                // Enregistrez le temps actuel
                tempsEnPause = DateTime.Now;

                // Bloquez les déplacements du joueur en désactivant la gestion des touches
                this.KeyDown -= Window_KeyDown;

                // Mettez en pause les déplacements des projectiles
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Pause(); // Cela semble être une méthode invalide, probablement une coquille.
                }
            }
            else
            {
                // Reprenez le jeu avec le temps initial
                timer.Start();
                jeuEnPause = false;

                // Calculez la différence de temps écoulée pendant la pause
                TimeSpan tempsPause = DateTime.Now - tempsEnPause;

                // Ajoutez le temps de pause au temps initial
                tempsLimite = tempsLimite.Add(tempsPause);

                // Activez à nouveau la gestion des touches
                this.KeyDown += Window_KeyDown;

                // Reprenez les déplacements des projectiles
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Reprendre(); // Cela semble être une méthode invalide, probablement une coquille.
                }
            }
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

            // Vérifier les touches en diagonale
            if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Right)) // Diagonale en haut à droite
            {
                if (Canvas.GetTop(personnage) - deplacement >= 0 && Canvas.GetLeft(personnage) + personnage.ActualWidth + deplacement <= canvas.ActualWidth)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) - deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Left)) // Diagonale en haut à gauche
            {
                if (Canvas.GetTop(personnage) - deplacement >= 0 && Canvas.GetLeft(personnage) - deplacement >= 0)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) - deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Right)) // Diagonale en bas à droite
            {
                if (Canvas.GetTop(personnage) + personnage.ActualHeight + deplacement <= canvas.ActualHeight && Canvas.GetLeft(personnage) + personnage.ActualWidth + deplacement <= canvas.ActualWidth)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) + deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Left)) // Diagonale en bas à gauche
            {
                if (Canvas.GetTop(personnage) + personnage.ActualHeight + deplacement <= canvas.ActualHeight && Canvas.GetLeft(personnage) - deplacement >= 0)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) + deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - deplacement);
                }
            }
        }



        private void AfficherMessageVictoireOuDefaite(string message)
        {
            // Arrêtez tous les timers ici si nécessaire
            timer.Stop();
            spawnTimer.Stop();

            // Affichez la boîte de dialogue VictoireDefaiteDialog
            VictoireDefaiteDialog dialog = new VictoireDefaiteDialog(message);
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                
                RejouerNiveau();
            }
            else
            {
               
                RetourMenu();
            }
        }
        private void RejouerNiveau()
        {
            ReinitialiserJeu();
        }

        private void RetourMenu()
        {
            // Cachez la fenêtre actuelle
            this.Hide();

            fenetreChoix.ShowDialog();

            if (fenetreChoix.DialogResult == true)
            {
                // Obtenez le nouveau niveau sélectionné
                difficulteActuelle = fenetreChoix.DifficulteChoisie;

                // Réinitialisez le jeu avec le nouveau niveau
                ReinitialiserJeu();

                // Affichez la fenêtre actuelle
                this.Show();
            }
            else
            {
                // Gérez le cas où l'utilisateur annule la sélection du niveau ou ferme la fenêtre de sélection
                Application.Current.Shutdown();
            }
        }

        public void InitialiserAvecNiveau(NiveauDifficulte niveau)
        {
            difficulteActuelle = niveau;
            ReinitialiserJeu();  // Ajoutez cette méthode pour réinitialiser le jeu avec le nouveau niveau
        }


        public class Projectile
        {
            private bool enPause;
            private Canvas canvas;
            private Rectangle projectileRect;
            private double speed = 40; // Vitesse du projectile
            
            public Projectile(Canvas canvas, Random random, string imagePath)
            {
                this.canvas = canvas;
                ImageBrush imageEnnemis = new ImageBrush();

                projectileRect = new Rectangle
                {
                    Width = 250,
                    Height = 250,
                    Fill = imageEnnemis
                };

                imageEnnemis.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

                // Add the projectile to the Canvas
                canvas.Children.Add(projectileRect);

                // Position the projectile randomly
                Canvas.SetLeft(projectileRect, random.Next(0, (int)canvas.ActualWidth));
                Canvas.SetTop(projectileRect, random.Next(0, (int)canvas.ActualHeight));
            }

            public void Move(double playerLeft, double playerTop, List<Projectile> otherProjectiles)
            {
                if (!enPause)  // Vérifiez si le projectile n'est pas en pause
                {
                    // Calculez la direction du déplacement
                    double deltaX = playerLeft - Canvas.GetLeft(projectileRect);
                    double deltaY = playerTop - Canvas.GetTop(projectileRect);

                    // Normalisez la direction
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double directionX = deltaX / length;
                    double directionY = deltaY / length;

                    // Déplacez le projectile dans la direction normalisée
                    double newLeft = Canvas.GetLeft(projectileRect) + directionX * speed;
                    double newTop = Canvas.GetTop(projectileRect) + directionY * speed;

                    //Limite Canvas
                    double canvasWidth = canvas.ActualWidth;
                    double canvasHeight = canvas.ActualHeight;

                    newLeft = Math.Max(0, Math.Min(newLeft, canvasWidth - projectileRect.Width));
                    newTop = Math.Max(0, Math.Min(newTop, canvasHeight - projectileRect.Height));

                    // Vérifiez s'il y a une collision avec d'autres projectiles
                    if (!CheckCollision(otherProjectiles, newLeft, newTop))
                    {
                        Canvas.SetLeft(projectileRect, newLeft);
                        Canvas.SetTop(projectileRect, newTop);
                    }
                }

            }



            // Méthode pour vérifier les collisions avec d'autres projectiles
            private bool CheckCollision(List<Projectile> otherProjectiles, double newLeft, double newTop)
            {
                Rect newRect = new Rect(newLeft, newTop, projectileRect.Width, projectileRect.Height);

                foreach (var otherProjectile in otherProjectiles)
                {
                    if (this != otherProjectile)
                    {
                        Rect otherRect = new Rect(Canvas.GetLeft(otherProjectile.projectileRect), Canvas.GetTop(otherProjectile.projectileRect), otherProjectile.projectileRect.Width, otherProjectile.projectileRect.Height);

                        if (newRect.IntersectsWith(otherRect))
                        {
                            return true; // Collision détectée
                        }
                    }
                }

                return false; // Pas de collision
            }
            public void Pause()
            {
                // Mettez le projectile en pause
                enPause = true;
            }

            public void Reprendre()
            {
                // Reprenez le projectile
                enPause = false;
            }
            public Rectangle GetRectangle()
            {
                return projectileRect;
            }
        }
        private void GenerateProjectile()
        {
            if (projectiles.Count < maxProjectiles)
            {
                string imagePath = "";

                // Choisissez l'image appropriée en fonction du niveau actuel
                switch (difficulteActuelle)
                {
                    case NiveauDifficulte.Apprenti:
                        imagePath = AppDomain.CurrentDomain.BaseDirectory + "img\\ennemis_naruto.png";
                        break;
                    case NiveauDifficulte.Amateur:
                        imagePath = AppDomain.CurrentDomain.BaseDirectory + "img\\ennemis_luffy.png";
                        break;
                    case NiveauDifficulte.Pro:
                        imagePath = AppDomain.CurrentDomain.BaseDirectory + "img\\ennemis_goku.png";
                        break;
                        // Ajoutez d'autres cas si nécessaire pour d'autres niveaux
                }

                Projectile projectile = new Projectile(canvas, random2, imagePath);
                projectiles.Add(projectile);
            }
        }
        private void SupprimerObjetsCollectes()
        {
            for (int i = 0; i < objetsCollectes; i++)
            {
                Image objetCollecte = canvas.Children.OfType<Image>().LastOrDefault();
                if (objetCollecte != null)
                {
                    canvas.Children.Remove(objetCollecte);
                }
            }
            objetsCollectes = 0;
        }

        private void ReinitialiserJeu()
        {
            // Supprimez les projectiles existants
            foreach (Projectile projectile in projectiles)
            {
                canvas.Children.Remove(projectile.GetRectangle());
            }
            projectiles.Clear();

            SupprimerObjetsCollectes();
            // Réinitialisez le compteur d'objets collectés
            objetsCollectes = 0;
            objetsTextBlock.Text = "Objets : 0/20";

            // Réinitialisez le compteur de vies
            viesRestantes = 3;
            vieTextBlock.Text = $"Vie : {viesRestantes}/3";

            // Réinitialisez le temps
            tempsLimite = DateTime.Now.AddMinutes(2);
            timer.Start();

            // Réinitialisez le message TextBlock s'il existe
            if (canvas.Children.Contains(messageTextBlock))
            {
                canvas.Children.Remove(messageTextBlock);
                messageTextBlock = null;
            }

            // Réinitialisez le personnage
            Canvas.SetLeft(personnage, 0);
            Canvas.SetTop(personnage, 0);

            // Supprimez l'image de fond du niveau
            foreach (Image imageFond in canvas.Children.OfType<Image>())
            {
                // Vérifiez s'il s'agit de l'image de fond du niveau
                if (imageFond.Source != null && imageFond.Source.ToString().Contains("fond"))
                {
                    canvas.Children.Remove(imageFond);
                    break;  // Sortez après la suppression de la première image de fond
                }
            }

            Image pauseImage = new Image();
            pauseImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\Pause_Button_Icon.png", UriKind.RelativeOrAbsolute));
            pauseImage.Width = 100;
            pauseImage.Height = 100;

            // Gérez l'événement Click ou MouseDown de l'image
            pauseImage.Tag = "PauseButton";
            pauseImage.MouseDown += PauseImage_MouseDown; 

            // Ajoutez l'image au Canvas
            canvas.Children.Add(pauseImage);

            Canvas.SetLeft(pauseImage, 1320);
            Canvas.SetTop(pauseImage, 10);
            // Redémarrez le timer de spawn
            spawnTimer.Start();

            // Affichez les objets à nouveau
            AjouterImageDuNiveau(difficulteActuelle);

 
        }

    }

}
