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
        
        private Canvas canvas;
        private Rectangle personnage;
        private int objetsCollectes;
        private Random random;

        private TextBlock tempsTextBlock;
        private TextBlock objetsTextBlock;

        private DateTime tempsLimite;
        private DateTime tempsEcoule;

        private NiveauDifficulte difficulteActuelle;

        private DispatcherTimer timer;
        private DispatcherTimer spawnTimer;

        private TextBlock messageTextBlock;
        private TextBlock vieTextBlock;

        private List<Projectile> projectiles;
        private Random random2;
        private int maxProjectiles = 1;

        private Button pauseButton;
        private bool jeuEnPause = false;
        private DateTime tempsEnPause;

        private int viesRestantes;

        private ChoixNiveau fenetreChoix;
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


                // INITIALISATION DU CANVAS
                
                canvas = new Canvas();
                this.Content = canvas;

                //NOMBRES ALEATOIRES D'OBJETS A RAMASSER
                random = new Random();

                //APPARITION ALEATOIRES DE L'ENNEMIS
                random2 = new Random();

                // INITIALISATION DE LA LISTE DES PROJECTILES
                projectiles = new List<Projectile>();

                // INITIALISATION DU NOMBRE DE VIES
                viesRestantes = 3;

                //FENETRE CHOIX
                fenetreChoix = new ChoixNiveau();

                // AJOUT IMAGE CANVAS
                string cheminImage = ObtenirCheminImageDuNiveau(difficulteActuelle);
                ImageBrush fondImage = new ImageBrush(new BitmapImage(new Uri(cheminImage, UriKind.RelativeOrAbsolute)));
                canvas.Background = fondImage;

                // CREATION DU PERSONNAGE
                personnage = new Rectangle();
                personnage.Width = 120;
                personnage.Height = 140;
                personnage.Fill = Brushes.Red;
                Canvas.SetLeft(personnage, 680);
                Canvas.SetTop(personnage, 100);

                // AJOUT DU PERSO AU CANVAS
                canvas.Children.Add(personnage);

                // INITIALISATION COMPTEUR OBJETS
                objetsCollectes = 0;

                // CONFIGURATION DU TIMER
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                // CONFIGURATION DU TIMER POUR L'APPARITTION DES OBJETS ALEATOIRES
                spawnTimer = new DispatcherTimer();
                spawnTimer.Interval = TimeSpan.FromSeconds(0.2); 
                spawnTimer.Tick += SpawnTimer_Tick;

                // TEXT-BLOCK COMPTEUR DE TEMPS
                tempsTextBlock = new TextBlock();
                tempsTextBlock.FontSize = 48;
                tempsTextBlock.Foreground = Brushes.Red;
                tempsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                tempsTextBlock.FontWeight = FontWeights.Bold;
                Canvas.SetTop(tempsTextBlock, 2);
                Canvas.SetLeft(tempsTextBlock, 640);
                canvas.Children.Add(tempsTextBlock);

                // TEXT-BLOCK COMPTEUR D'OBJETS
                objetsTextBlock = new TextBlock();
                objetsTextBlock.FontSize = 30;
                objetsTextBlock.Foreground = Brushes.Blue;
                objetsTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                objetsTextBlock.FontFamily = new FontFamily("Showcard Gothic");
                Canvas.SetTop(objetsTextBlock, 10);
                canvas.Children.Add(objetsTextBlock);

                //TEXT-BLOCK COMPTEUR DE VIE
                vieTextBlock = new TextBlock();
                vieTextBlock.FontSize = 30;
                vieTextBlock.Foreground = Brushes.Purple;
                vieTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                vieTextBlock.FontFamily = new FontFamily("Showcard Gothic");

                // INITIALISER LE COMPTEUR DE VIE AU DEBUT
                vieTextBlock.Text = $"Vie : {viesRestantes}/3";
                Canvas.SetTop(vieTextBlock, 60);
                Canvas.SetLeft(vieTextBlock, 10);
                canvas.Children.Add(vieTextBlock);

                //BOUTON PAUSE
                Image pauseImage = new Image();
                pauseImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\bouton_pause.png", UriKind.RelativeOrAbsolute));
                pauseImage.Width = 50;
                pauseImage.Height = 50;


                //EVENEMENT CLICK SUR BOUTON PAUSE
                pauseImage.Tag = "PauseButton";
                pauseImage.MouseDown += PauseImage_MouseDown; 

                // AJOUT DE L'IMAGE AU CANVAS
                canvas.Children.Add(pauseImage);
                Canvas.SetLeft(pauseImage, 1320);
                Canvas.SetTop(pauseImage, 10);

                //LANCEMENT DU NIVEAU
                InitialiserNiveau();
            }
            
        }
        
        //EXECUTION DU NIVEAU
        private void InitialiserNiveau()
        {
            ExecuterNiveau();
        }


        //EXECUTION DU TIMER A 2 MINUTES
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        { 
            tempsLimite = DateTime.Now.AddSeconds(120);
            timer.Start();
        }

        //GERE LA PARTIE TIMER
        private void Timer_Tick(object sender, EventArgs e)
        {
            tempsEcoule = DateTime.Now;

            TimeSpan tempsRestant = tempsLimite - tempsEcoule;
            tempsTextBlock.Text = $" {tempsRestant.Minutes:00}:{tempsRestant.Seconds:00}";

            if (tempsEcoule >= tempsLimite)
            {
                timer.Stop();
                AfficherMessageVictoireOuDefaite("Temps écoulé. Vous avez perdu !");
            }
        }

        //EVENEMENT TICK
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

                        if (objetsCollectes >= 20)
                        {
                            timer.Stop();
                            spawnTimer.Stop();
                            AfficherMessageVictoireOuDefaite("Vous avez gagné !");
                            return;
                            // Sortez de la méthode après la victoire
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

            // VERIFIE SI LE NOMBRE D'OBJETS COLLECTES EST EGALE A 20
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
                GenerateProjectile();
            }

        }

        private bool IsCollision(double left1, double top1, double width1, double height1,double left2, double top2, double width2, double height2, bool isProjectile = false)
        {
            if ((left1 + width1) < left2 || left1 > (left2 + width2) ||
                (top1 + height1) < top2 || top1 > (top2 + height2))
            {
                return false;
            }
            else
            {
                if (!isProjectile)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public class GameObject
        {
            public Image Image { get; set; }
            public DispatcherTimer Timer { get; set; }
        }

        //EXECUTION DU NIVEAU
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

        //AJOUT DE L4IMAGE DE FOND
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

        //REDIRECTION VERS LE NIVEAU CHOISI
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

        //EVENEMENT PAUSE
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
                    projectile.Pause(); 
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
                    projectile.Reprendre();
                }
            }
        }


        //DEPLACEMENT DU PERSONNAGE
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

            if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Right)) 
            {
                if (Canvas.GetTop(personnage) - deplacement >= 0 && Canvas.GetLeft(personnage) + personnage.ActualWidth + deplacement <= canvas.ActualWidth)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) - deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Left))
            {
                if (Canvas.GetTop(personnage) - deplacement >= 0 && Canvas.GetLeft(personnage) - deplacement >= 0)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) - deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Right))
            {
                if (Canvas.GetTop(personnage) + personnage.ActualHeight + deplacement <= canvas.ActualHeight && Canvas.GetLeft(personnage) + personnage.ActualWidth + deplacement <= canvas.ActualWidth)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) + deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) + deplacement);
                }
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.Left))
            {
                if (Canvas.GetTop(personnage) + personnage.ActualHeight + deplacement <= canvas.ActualHeight && Canvas.GetLeft(personnage) - deplacement >= 0)
                {
                    Canvas.SetTop(personnage, Canvas.GetTop(personnage) + deplacement);
                    Canvas.SetLeft(personnage, Canvas.GetLeft(personnage) - deplacement);
                }
            }
        }

        //ENNEMIS
        public class Projectile
        {
            private bool enPause;
            private Canvas canvas;
            private Rectangle projectileRect;
            private double speed = 40;

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
                canvas.Children.Add(projectileRect);
                Canvas.SetLeft(projectileRect, random.Next(0, (int)canvas.ActualWidth));
                Canvas.SetTop(projectileRect, random.Next(0, (int)canvas.ActualHeight));
            }

            //MOUVEMENT DE L'ENNEMI
            public void Move(double playerLeft, double playerTop, List<Projectile> otherProjectiles)
            {
                if (!enPause)
                {
                    double deltaX = playerLeft - Canvas.GetLeft(projectileRect);
                    double deltaY = playerTop - Canvas.GetTop(projectileRect);

                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double directionX = deltaX / length;
                    double directionY = deltaY / length;

                    double newLeft = Canvas.GetLeft(projectileRect) + directionX * speed;
                    double newTop = Canvas.GetTop(projectileRect) + directionY * speed;

                    double canvasWidth = canvas.ActualWidth;
                    double canvasHeight = canvas.ActualHeight;

                    newLeft = Math.Max(0, Math.Min(newLeft, canvasWidth - projectileRect.Width));
                    newTop = Math.Max(0, Math.Min(newTop, canvasHeight - projectileRect.Height));

                    if (!CheckCollision(otherProjectiles, newLeft, newTop))
                    {
                        Canvas.SetLeft(projectileRect, newLeft);
                        Canvas.SetTop(projectileRect, newTop);
                    }
                }
            }

            //VERIFIE LES COLLISIONS AVEC L'ENNEMI
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
                            return true;
                        }
                    }
                }

                return false;
            }

            //PAUSE
            public void Pause()
            {
                enPause = true;
            }

            //REPRENDRE
            public void Reprendre()
            {
                enPause = false;
            }
            public Rectangle GetRectangle()
            {
                return projectileRect;
            }
        }

        //GENERATION DE L'ENNEMI
        private void GenerateProjectile()
        {
            if (projectiles.Count < maxProjectiles)
            {
                string imagePath = "";

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
                }

                Projectile projectile = new Projectile(canvas, random2, imagePath);
                projectiles.Add(projectile);
            }
        }


        //MESSAGE DE VICTOIRE OU DE DEFAITE
        private void AfficherMessageVictoireOuDefaite(string message)
        {
            timer.Stop();
            spawnTimer.Stop();

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

        //BOUTON REJOUER QUI REINITIALISE LE NIVEAU
        private void RejouerNiveau()
        {
            ReinitialiserJeu();
        }

        //BOUTON RETOUR MENU QUI RENVOIE VERS LE CHOIX DES NIVEAUX
        private void RetourMenu()
        {
            this.Hide();

            fenetreChoix.ShowDialog();

            if (fenetreChoix.DialogResult == true)
            {
                difficulteActuelle = fenetreChoix.DifficulteChoisie;

                ReinitialiserJeu();

                this.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        //NOUVEAU CHOIX DU NIVEAU
        public void InitialiserAvecNiveau(NiveauDifficulte niveau)
        {
            difficulteActuelle = niveau;
            ReinitialiserJeu(); 
        }

        

        //SUPPRIMME LES OBJETS COLLECTES
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

        //REINTIALISATION DU JEU
        private void ReinitialiserJeu()
        {
            foreach (Projectile projectile in projectiles)
            {
                canvas.Children.Remove(projectile.GetRectangle());
            }
            projectiles.Clear();

            SupprimerObjetsCollectes();
            objetsCollectes = 0;
            objetsTextBlock.Text = "Objets : 0/20";

            viesRestantes = 3;
            vieTextBlock.Text = $"Vie : {viesRestantes}/3";

            tempsLimite = DateTime.Now.AddMinutes(2);
            timer.Start();

            if (canvas.Children.Contains(messageTextBlock))
            {
                canvas.Children.Remove(messageTextBlock);
                messageTextBlock = null;
            }

            Canvas.SetLeft(personnage, 0);
            Canvas.SetTop(personnage, 0);

            foreach (Image imageFond in canvas.Children.OfType<Image>())
            {
                if (imageFond.Source != null && imageFond.Source.ToString().Contains("fond"))
                {
                    canvas.Children.Remove(imageFond);
                    break;
                }
            }

            Image pauseImage = new Image();
            pauseImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img\\bouton_pause.png", UriKind.RelativeOrAbsolute));
            pauseImage.Width = 100;
            pauseImage.Height = 100;

            pauseImage.Tag = "PauseButton";
            pauseImage.MouseDown += PauseImage_MouseDown; 


            canvas.Children.Add(pauseImage);

            Canvas.SetLeft(pauseImage, 1320);
            Canvas.SetTop(pauseImage, 10);
            spawnTimer.Start();

            AjouterImageDuNiveau(difficulteActuelle);

 
        }

    }

}
