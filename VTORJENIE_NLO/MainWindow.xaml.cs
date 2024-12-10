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
using System.Media;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using VTORJENIE_NLO.Properties;

namespace VTORJENIE_NLO
{
    public class Wave
    {
        public int WaveSeconds;
        public int EnemyHP;
        public int SpawnerTick;
        public int WaveNumber = 1;
        

        public Wave()
        {
            WaveSeconds = 15;
            EnemyHP = 3;
            SpawnerTick = 2750;
        }

        public void nextWave()
        {
            if (WaveSeconds < 180)
                WaveSeconds += 5;

            if (EnemyHP < 90)
                EnemyHP += 3;

            if (SpawnerTick > 250)
                SpawnerTick -= 250;

            WaveNumber++;
        }

    }



    public class InputAxis
    {
        public bool W, A, S, D, J;

        public InputAxis()
        {
            W = false;
            A = false;
            S = false;
            D = false;
            J = false;
        }
    }


    public class Coin
    {
        private BitmapImage SPRITE;
        public Image COIN;
        private Canvas CANVAS;
        public double X, Y;
        public Coin(Canvas GameCanvas, double tX, double tY)
        {
            X = tX;
            Y = tY;
            SPRITE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\coin.png"));
            COIN = new Image
            {
                Source = SPRITE,
                Width = 15,
                Height = 15,
                RenderTransform = new TranslateTransform()
            };
            RenderOptions.SetBitmapScalingMode(COIN, BitmapScalingMode.NearestNeighbor);
            COIN.Visibility = Visibility.Hidden;
            CANVAS = GameCanvas;
        }

        public void CoinSpawn()
        {
            Canvas.SetLeft(COIN, X);
            Canvas.SetTop(COIN, Y);
            COIN.Visibility = Visibility.Visible;
            CANVAS.Children.Add(COIN);
        }

        public void moveToUfo(double x, double y, Ufo UFO)
        {

            double deltaX = x - Canvas.GetLeft(COIN);
            double deltaY = y - Canvas.GetTop(COIN);

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > 0 && distance < 200)
            {
                double step = 1;
                double moveX = step * (deltaX / distance);
                double moveY = step * (deltaY / distance);

                Canvas.SetLeft(COIN, Canvas.GetLeft(COIN) + moveX);
                Canvas.SetTop(COIN, Canvas.GetTop(COIN) + moveY);
            }

            X = Canvas.GetLeft(COIN);
            Y = Canvas.GetTop(COIN);

        }
    }
        public class Laser
    {
        private BitmapImage SPRITE;
        private Image LASER;
        private Canvas CANVAS;
        public Laser(Canvas GameCanvas)
        {
            SPRITE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\laser.png"));
            LASER = new Image
            {
                Source = SPRITE,
                Width = 30,
                Height = 30,
                RenderTransform = new TranslateTransform()
            };
            RenderOptions.SetBitmapScalingMode(LASER, BitmapScalingMode.NearestNeighbor);
            GameCanvas.Children.Add(LASER);
            LASER.Visibility = Visibility.Hidden;
            CANVAS = GameCanvas;
        }
        
        public void shoot(double EnemyX, double EnemyY, double UfoX, double UfoY, List<Enemy> ENEMIES, Canvas CANVAS, List<Coin> COINS)
        {
            double angle = Math.Atan2(EnemyY - UfoY, EnemyX- UfoX);

            double speed = 10;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);

            double currentX = UfoX;
            double currentY = UfoY;

            Canvas.SetLeft(LASER, currentX);
            Canvas.SetTop(LASER, currentY);

            LASER.Visibility = Visibility.Visible;

            timer.Tick += (s, e) =>
            {
                double nextX = currentX + Math.Cos(angle) * speed;
                double nextY = currentY + Math.Sin(angle) * speed;

                Canvas.SetLeft(LASER, nextX);
                Canvas.SetTop(LASER, nextY);

                currentX = nextX;
                currentY = nextY;

                foreach (Enemy enemy in ENEMIES)
                {
                    if (Math.Abs(nextX - enemy.X) < 30 && Math.Abs(nextY - enemy.Y) < 30)
                    {
                        enemy.HP--;

                        Label damageLabel = new Label();
                        damageLabel.Content = "-1";
                        damageLabel.FontSize = 20;
                        damageLabel.FontFamily = new FontFamily(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\Monocraft.otf#Monocraft");
                        damageLabel.Foreground = Brushes.White;
                        Canvas.SetLeft(damageLabel, enemy.X);
                        Canvas.SetTop(damageLabel, enemy.Y - 20);
                        CANVAS.Children.Add(damageLabel);
                        CANVAS.Children.Remove(LASER);

                        DispatcherTimer damageTimer = new DispatcherTimer();
                        damageTimer.Interval = TimeSpan.FromMilliseconds(100);
                        damageTimer.Tick += (s1, e1) =>
                        {
                            CANVAS.Children.Remove(damageLabel);
                            damageTimer.Stop();
                        };

                        damageTimer.Start();
                        timer.Stop();

                        if (enemy.HP <= 0)
                        {
                            CANVAS.Children.Remove(enemy.ENEMY);
                            ENEMIES.Remove(enemy);
                            Coin COIN = new Coin(CANVAS, nextX, nextY);
                            COIN.CoinSpawn();
                            COINS.Add(COIN);

                        }

                        break;
                    }


                    if (nextX > 1270 || nextX < 0 || nextY > 710 || nextY < -20)
                    {
                        CANVAS.Children.Remove(LASER);
                        timer.Stop();
                    }

                }
            };


            timer.Start();
            
        }

    }

    public class Enemy
    {
        private BitmapImage SPRITE;
        public Image ENEMY;
        public double X, Y;
        public double HP;
        public Enemy(Canvas GameCanvas)
        {
            SPRITE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\enemy.png"));
            ENEMY = new Image
            {
                Source = SPRITE,
                Width = 60,
                Height = 60,
                RenderTransform = new TranslateTransform()
            };

            HP = 3;

            Random rnd = new Random();
            int left = rnd.Next(-1, 1);
            int top = rnd.Next(-1, 1);

            if (left >= 0)
                Canvas.SetLeft(ENEMY, 1270);
            else
                Canvas.SetLeft(ENEMY, 0);

            if (top >= 0)
                Canvas.SetTop(ENEMY, 710);
            else
                Canvas.SetTop(ENEMY, -20);

            RenderOptions.SetBitmapScalingMode(ENEMY, BitmapScalingMode.NearestNeighbor);
            GameCanvas.Children.Add(ENEMY);
        }

        public void moveToUfo(double x, double y, Ufo UFO)
        {

            double deltaX = x - Canvas.GetLeft(ENEMY);
            double deltaY = y - Canvas.GetTop(ENEMY);

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > 0)
            {
                double step = 1;
                double moveX = step * (deltaX / distance);
                double moveY = step * (deltaY / distance);

                Canvas.SetLeft(ENEMY, Canvas.GetLeft(ENEMY) + moveX);
                Canvas.SetTop(ENEMY, Canvas.GetTop(ENEMY) + moveY);
            }

            X = Canvas.GetLeft(ENEMY);
            Y = Canvas.GetTop(ENEMY);

        }
    }

    public class Ufo
    {
        private Image SPRITE;
        private Canvas CANVAS;
        public int HP, MAXHP, HPREGEN, DEF, SPEED, BULLETDMG, UFODMG, BULLETCOOLDOWN, DAMAGECOOLDOWN, COINS;
        public bool INBULLET;
        public double X, Y;
        public bool isAlive, isDamaged;

        public string UFO_LEFT_SPRITE, UFO_UP_LEFT_SPRITE, UFO_DOWN_LEFT_SPRITE, UFO_RIGHT_SPRITE,
            UFO_UP_RIGHT_SPRITE, UFO_DOWN_RIGHT_SPRITE, UFO_UP_SPRITE, UFO_DOWN_SPRITE, UFO_IDLE_SPRITE;

        public Ufo(Image ufoSprite, Canvas ufoCanvas)
        {
            SPRITE = ufoSprite;
            CANVAS = ufoCanvas;
            isAlive = true;
            isDamaged = false;

            COINS = 0;
            HP = 3;
            MAXHP = 3;
            HPREGEN = 0;
            DEF = 0;
            SPEED = 400;
            BULLETDMG = 1;
            UFODMG = 1;
            BULLETCOOLDOWN = 100;
            DAMAGECOOLDOWN = 500;

            INBULLET = false;
            UFO_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left.png";
            UFO_UP_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left-up.png";
            UFO_DOWN_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left-down.png";
            UFO_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right.png";
            UFO_UP_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right-up.png";
            UFO_DOWN_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right-down.png";
            UFO_UP_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-up.png";
            UFO_DOWN_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-down.png";
            UFO_IDLE_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\logo.png";
            X = Canvas.GetLeft(SPRITE);
            Y = Canvas.GetTop(SPRITE);
        }

        public async void Damage()
        {
            if (isDamaged) return;
            
            isDamaged = true;

            HP--;

            await Task.Delay(DAMAGECOOLDOWN);

            isDamaged = false;
        }

        public Point getUfoCenter()
        {
            double centerX = Canvas.GetLeft(SPRITE) + SPRITE.Width / 2;
            double centerY = Canvas.GetTop(SPRITE) + SPRITE.Height / 2;
            Console.WriteLine(centerX);
            return new Point(centerX, centerY);
        }

        public async void shoot(List<Enemy> ENEMIES, List<Coin> COINS)
        {
            if (ENEMIES.Count == 0 || INBULLET) return;

            INBULLET = true;
            
            double enemyX = ENEMIES[0].X;
            double enemyY = ENEMIES[0].Y;
            
            double minDistanceSquared = Math.Pow(ENEMIES[0].X - getUfoCenter().X, 2) + Math.Pow(ENEMIES[0].Y - getUfoCenter().Y, 2);
            
            foreach (Enemy enemy in ENEMIES) 
            {
                double distanceSquared = Math.Pow(enemy.X - getUfoCenter().X, 2) + Math.Pow(enemy.Y - getUfoCenter().Y, 2);

                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    enemyX = enemy.X;
                    enemyY = enemy.Y;
                }
                
            }

            Laser LASER = new Laser(CANVAS);

            LASER.shoot(enemyX, enemyY, getUfoCenter().X, getUfoCenter().Y, ENEMIES, CANVAS, COINS);

            await Task.Delay(BULLETCOOLDOWN);

            INBULLET = false;
        }

        public async void DeathAnimation()
        {
            SPRITE.Visibility = Visibility.Hidden;

            BitmapImage ExplosionSPRITE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\EXPLOSION1.png"));
            Image EXPLOSION = new Image
            {
                Source = ExplosionSPRITE,
                Width = 200,
                Height = 200,
            };

            Canvas.SetLeft(EXPLOSION, getUfoCenter().X-100);
            Canvas.SetTop(EXPLOSION, getUfoCenter().Y-100);
            Canvas.SetZIndex(EXPLOSION, 2);
            RenderOptions.SetBitmapScalingMode(EXPLOSION, BitmapScalingMode.NearestNeighbor);
            CANVAS.Children.Add(EXPLOSION);
            for (int i = 1; i <= 6; i++)
            {
                EXPLOSION.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"..\\..\\IMG\\EXPLOSION{i}.png"));
                await Task.Delay(200);
            }
            CANVAS.Children.Remove(EXPLOSION);
            isAlive = false;
        }

        public void move(double directionX, double directionY)
        {
            double newX, newY;

            newX = Canvas.GetLeft(SPRITE) + SPEED * directionX;
            newY = Canvas.GetTop(SPRITE) + SPEED * directionY;

            newX = newX < 0 ? 0 : newX;
            newX = newX > CANVAS.Width - SPRITE.Width ? CANVAS.Width - SPRITE.Width : newX;

            newY = newY < -20 ? -20 : newY;
            newY = newY > CANVAS.Height - SPRITE.Height ? CANVAS.Height - SPRITE.Height : newY;

            Canvas.SetLeft(SPRITE, newX);
            Canvas.SetTop(SPRITE, newY);

            X = Canvas.GetLeft(SPRITE);
            Y = Canvas.GetTop(SPRITE);
        }

        public void shopContact(List<Image> pivos)
        {
            foreach (Image pivo in pivos)
            {
                if (Math.Abs(Canvas.GetLeft(pivo) - getUfoCenter().X) <= 50)
                {
                    Label PRICE = new Label();
                    PRICE.Content = "-1";
                    PRICE.FontSize = 20;
                    PRICE.FontFamily = new FontFamily(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\Monocraft.otf#Monocraft");
                    PRICE.Foreground = Brushes.White;
                    CANVAS.Children.Add(PRICE);
                }
            }
        }
    }


    public partial class MainWindow : Window
    {
        private SoundPlayer bgMusicSFX = new SoundPlayer(Properties.Resources.background);
        private MediaPlayer alienClickSFX = new MediaPlayer();
        private MediaPlayer uiClickSFX = new MediaPlayer();

        private MediaPlayer coinCollectSFX = new MediaPlayer();
        private MediaPlayer laserShootSFX = new MediaPlayer();

        private MediaPlayer takeDamgeSFX = new MediaPlayer();
        private MediaPlayer explosionSFX = new MediaPlayer();


        private DispatcherTimer timer = null;
        private DispatcherTimer EnemySpawnTimer = null;
        private DispatcherTimer EnemyTurnTimer = null;
        private DispatcherTimer CoinTimer = null;
        private DispatcherTimer WaveTimer = null;
        private DateTime lastUpdateTime;
        
        private Ufo UFO;
        private Wave WAVE;
        private List<Enemy> ENEMIES;
        private List<Coin> COINS;
        private InputAxis INPUTS;
        private List<Image> PIVOS;

        private const string URL = "https://github.com/snezh0ks/VTORJENIE_NLO";
        private bool START;
        private double directionX = 0, directionY = 0;

        public MainWindow()
        {
            InitializeComponent();
            bgMusicPlay();
            alienAnimationPlay();
            pivoAnimationPlay();
        }

        private void pivoAnimationPlay()
        {

            DoubleAnimation animation = new DoubleAnimation
            {
                From = -10,
                To = 10,
                Duration = new Duration(TimeSpan.FromSeconds(3.0)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Pivo1RotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
            Pivo2RotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
            Pivo3RotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }


        private void alienAnimationPlay()
        {
            // анимация инопланетянина в главном меню

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 20,
                Duration = new Duration(TimeSpan.FromSeconds(3.0)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            alienTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void bgAnimationPlay()
        {
            // анимация заднего фона игры

            DoubleAnimation animation1 = new DoubleAnimation
            {
                From = 0,
                To = -1280,
                Duration = new Duration(TimeSpan.FromSeconds(5)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            firstStarsTransform.BeginAnimation(TranslateTransform.XProperty, animation1);

            DoubleAnimation animation2 = new DoubleAnimation
            {
                From = 1280,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(5)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            secondStarsTransform.BeginAnimation(TranslateTransform.XProperty, animation2);
        }

        private void bgMusicPlay()
        {
            // фоновая музыка

            bgMusicSFX.PlayLooping();
        }

        private void alienClickPlay(object sender, RoutedEventArgs e)
        {
            // звуки злого инопланетянина

            alienClickSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\voice.wav"));
            alienClickSFX.Play();
        }

        private void coinCollectPlay()
        {


            coinCollectSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\coin.wav"));
            coinCollectSFX.Play();
        }

        private void takeDamgePlay()
        {
            takeDamgeSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\takedamage.wav"));
            takeDamgeSFX.Play();
        }

        private void explosionPlay()
        {
            explosionSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\explosion.wav"));
            explosionSFX.Play();
        }

        private void laserShootPlay()
        {
   

            laserShootSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\laser.wav"));
            laserShootSFX.Play();
        }

        private void UiClickPlay()
        {
            // звук клика

            uiClickSFX.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\ui.wav"));
            uiClickSFX.Play();
        }

        private void timerStart()
        {
            // запуск таймера

            alienClickSFX.Stop();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(1);
            timer.Start();

            EnemySpawnTimer = new DispatcherTimer();
            EnemySpawnTimer.Tick += new EventHandler(EnemySpawnTimer_Tick);
            EnemySpawnTimer.Interval = TimeSpan.FromMilliseconds(WAVE.SpawnerTick);
            EnemySpawnTimer.Start();

            EnemyTurnTimer = new DispatcherTimer();
            EnemyTurnTimer.Tick += new EventHandler(EnemyTurnTimer_Tick);
            EnemyTurnTimer.Interval = TimeSpan.FromMilliseconds(10);
            EnemyTurnTimer.Start();

            CoinTimer = new DispatcherTimer();
            CoinTimer.Tick += new EventHandler(CoinTimer_Tick);
            CoinTimer.Interval = TimeSpan.FromMilliseconds(10);
            CoinTimer.Start();

            WaveTimer = new DispatcherTimer();
            WaveTimer.Tick += new EventHandler(WaveTimer_Tick);
            WaveTimer.Interval = TimeSpan.FromSeconds(1);
            WaveTimer.Start();

            TimeLeftLabel.Content = Convert.ToString(WAVE.WaveSeconds);

            START = true;

            
        }

        private void WaveTimer_Tick(object sendedr, EventArgs e)
        {
            if (!START)
            {
                return;
            }

            TimeLeftLabel.Content = Convert.ToString(Convert.ToInt32(TimeLeftLabel.Content)-1);
            if (Convert.ToInt32(TimeLeftLabel.Content) <= 0)
            {
                ScreenClr();
                WaveTimer.Stop();
                WAVE.nextWave();
                WaveTimer = new DispatcherTimer();
                WaveTimer.Tick += new EventHandler(WaveTimer_Tick);
                WaveTimer.Interval = TimeSpan.FromSeconds(1);
                TimeLeftLabel.Content = Convert.ToString(Convert.ToInt32(WAVE.WaveSeconds) - 1);
                WaveTimer.Start();
                WaveNumber.Content = $"Волна {WAVE.WaveNumber}";
                //SHOP.Visibility = Visibility.Visible;
                //TimeLeftLabel.Visibility = Visibility.Hidden;

            }
        }

        private void ScreenClr()
        {
            foreach(Enemy enemy in ENEMIES)
            {
                ufoCanvas.Children.Remove(enemy.ENEMY);
            }
            ENEMIES.Clear();

            foreach (Coin coin in COINS)
            {
                ufoCanvas.Children.Remove(coin.COIN);
            }
            COINS.Clear();
        }

        private void CoinTimer_Tick(object sender, EventArgs e)
        {
            if (!START)
            {
                CoinTimer.Stop();
                return;
            }

            foreach (Coin COIN in COINS)
            {
                if (Math.Abs(COIN.X - UFO.getUfoCenter().X) < 30 && Math.Abs(COIN.Y - UFO.getUfoCenter().Y) < 30)
                {
                    UFO.COINS++;
                    COINSLABEL.Content = $"{UFO.COINS}";
                    ufoCanvas.Children.Remove(COIN.COIN);
                    COINS.Remove(COIN);
                    coinCollectPlay();
                    break;
                }

                COIN.moveToUfo(UFO.getUfoCenter().X, UFO.getUfoCenter().Y, UFO);

            }
        }

        private void EnemySpawnTimer_Tick(object sender, EventArgs e)
        {
            if (!START)
            {
                EnemySpawnTimer.Stop();
                return;
            }

            if (ENEMIES.Count <= 15) {

                ENEMIES.Add(new Enemy(ufoCanvas));
                ENEMIES[ENEMIES.Count - 1].HP = WAVE.EnemyHP;
            }

            
        }

        private void EnemyTurnTimer_Tick(object sender, EventArgs e)
        {
            if (!START)
            {
                EnemyTurnTimer.Stop();
                return;
            }

            DateTime currentTime = DateTime.Now;
            double deltaTime = (currentTime - lastUpdateTime).TotalSeconds;
            lastUpdateTime = currentTime;

            for (int i = 0; i < ENEMIES.Count; i++)
            {
                ENEMIES[i].moveToUfo(UFO.getUfoCenter().X, UFO.getUfoCenter().Y, UFO);

                if (Math.Abs(ENEMIES[i].X - UFO.getUfoCenter().X) < 30 && Math.Abs(ENEMIES[i].Y - UFO.getUfoCenter().Y) < 30)
                {
                    UFO.Damage();
                    if (UFO.HP > 0)
                        takeDamgePlay();
                    UFOHPLABEL.Content = $"{UFO.HP}/{UFO.MAXHP}";
                }

                if (UFO.HP <= 0)
                {
                    START = false;
                    explosionPlay();
                    UFO.DeathAnimation();
                }
            }

        }





        private void timer_Tick(object sender, EventArgs e)
        {
            if (!START && !UFO.isAlive)
            {
                timer.Stop();
                return;
            }

            // основной цикл игры

            DateTime currentTime = DateTime.Now;
            double deltaTime = (currentTime - lastUpdateTime).TotalSeconds;
            lastUpdateTime = currentTime;
            UFO.move(directionX * deltaTime, directionY * deltaTime);
            
            if (directionX > 0 && directionY > 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_DOWN_RIGHT_SPRITE));
            else if (directionX > 0 && directionY < 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_UP_RIGHT_SPRITE));
            else if (directionX > 0 && directionY == 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_RIGHT_SPRITE));
            else if (directionX < 0 && directionY < 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_UP_LEFT_SPRITE));
            else if (directionX < 0 && directionY > 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_DOWN_LEFT_SPRITE));
            else if (directionX < 0 && directionY == 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_LEFT_SPRITE));
            else if (directionX == 0 && directionY > 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_DOWN_SPRITE));
            else if (directionX == 0 && directionY < 0) ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_UP_SPRITE));
            else ufoSprite.Source = new BitmapImage(new Uri(UFO.UFO_IDLE_SPRITE));

            if (INPUTS.J)
            {
                laserShootPlay();
                UFO.shoot(ENEMIES, COINS);
            }

            
        }

        private void ChoiseDiff()
        {
            LEVELMENU.Visibility = Visibility.Hidden;
            GAME.Visibility = Visibility.Visible;

            UFO = new Ufo(ufoSprite, ufoCanvas);
            INPUTS = new InputAxis();
            lastUpdateTime = DateTime.Now;
            START = false;
            ENEMIES = new List<Enemy>();
            COINS = new List<Coin>();
            WAVE = new Wave();

            PIVOS = new List<Image>() {pivo1, pivo2, pivo3};

            timerStart();
            bgAnimationPlay();
        }

        private async void Diff1ButtonClick(object sender, RoutedEventArgs e)
        {
            UiClickPlay();
            await Task.Delay(500);
            ChoiseDiff();
        }

        private async void Diff2ButtonClick(object sender, RoutedEventArgs e)
        {
            UiClickPlay();
            await Task.Delay(500);
            ChoiseDiff();
        }

        private async void Diff3ButtonClick(object sender, RoutedEventArgs e)
        {
            UiClickPlay();
            await Task.Delay(500);
            ChoiseDiff();
        }
        private async void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            // кнопка запуска игры

            UiClickPlay();
            await Task.Delay(500);
            MENU.Visibility = Visibility.Hidden;
            LEVELMENU.Visibility = Visibility.Visible;
        }

        private async void OpenCodeButtonClick(object sender, RoutedEventArgs e)
        {
            // кнопка открытия исходного кода

            UiClickPlay();
            await Task.Delay(500);
            Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true });
        }

        private async void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            // кнопка выхода

            UiClickPlay();
            await Task.Delay(500);
            Environment.Exit(0);
        }

        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (MENU.Visibility == Visibility.Visible)
            {
                return;
            }

            if (LEVELMENU.Visibility == Visibility.Visible && e.Key == Key.Escape)
            {
                UiClickPlay();
                await Task.Delay(500);
                MENU.Visibility = Visibility.Visible;
                LEVELMENU.Visibility = Visibility.Hidden;
            }

            if (GAME.Visibility == Visibility.Visible && e.Key == Key.Escape)
            {
                UiClickPlay();
                await Task.Delay(500);

                ScreenClr();
                EnemySpawnTimer.Stop();
                EnemyTurnTimer.Stop();
                CoinTimer.Stop();
                WaveTimer.Stop();
                timer.Stop();
                UFO.HP = 3;
                START = false;
                UFOHPLABEL.Content = $"{UFO.HP}/{UFO.MAXHP}";
                WaveNumber.Content = $"Волна 1";
                ufoSprite.Visibility = Visibility.Visible;
                COINSLABEL.Content = "0";
                Canvas.SetLeft(ufoSprite, 30);
                Canvas.SetTop(ufoSprite, 300);

                LEVELMENU.Visibility = Visibility.Visible;
                GAME.Visibility = Visibility.Hidden;
            }

            // обработка лазера

            if (e.Key == Key.J)
            {
                INPUTS.J = true;
            }

            // обработка движения влево-вправо при нажатии клавиши

            if (!START && !UFO.isAlive) return;

            if (e.Key == Key.D)
            {
                directionX = 1;
                INPUTS.D = true;
            }

            else if (e.Key == Key.A)
            {
                directionX = -1;
                INPUTS.A = true;
            }

            // обработка движения вверх-вниз при нажатии клавиши

            if (e.Key == Key.W)
            {
                directionY = -1;
                INPUTS.W = true;
            }
            else if (e.Key == Key.S)
            {
                directionY = 1;
                INPUTS.S = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // обработка движения влево-вправо при отпускании клавиши

            if (MENU.Visibility == Visibility.Visible)
            {
                return;
            }

            if (!START && !UFO.isAlive) return;

            if (e.Key == Key.D)
            {
                INPUTS.D = false;
                if (INPUTS.A) directionX = -1;
                else directionX = 0;
            }

            else if (e.Key == Key.A)
            {
                INPUTS.A = false;
                if (INPUTS.D) directionX = 1;
                else directionX = 0;
            }

            // обработка движения влево-вправо при отпускании клавиши

            if (e.Key == Key.W)
            {
                INPUTS.W = false;
                if (INPUTS.S) directionY = 1;
                else directionY = 0;
            }

            else if (e.Key == Key.S)
            {
                INPUTS.S = false;
                if (INPUTS.W) directionY = -1;
                else directionY = 0;
            }

            // обработка лазера

            if (e.Key == Key.J)
            {
                INPUTS.J = false;
            }
        }
    }
}
