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

namespace VTORJENIE_NLO
{
    public class InputAxis
    {
        public bool W, A, S, D;

        public InputAxis()
        {
            W = false;
            A = false;
            S = false;
            D = false;
        }
    }

    public class Laser
    {
        private BitmapImage SPRITE;
        private Image LASER;
        public Laser(Canvas GameCanvas)
        {
            SPRITE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\laser.png"));
            LASER = new Image
            {
                Source = SPRITE,
                Width = 60,
                Height = 60,
                RenderTransform = new TranslateTransform()
            };
            RenderOptions.SetBitmapScalingMode(LASER, BitmapScalingMode.NearestNeighbor);
            GameCanvas.Children.Add(LASER);
        }
        

    }

    public class Ufo
    {
        private Image SPRITE;
        private Canvas CANVAS;
        public int SPEED, HP, BULLET;
        public double BULLETCOOLDOWN;
        
        public string UFO_LEFT_SPRITE, UFO_UP_LEFT_SPRITE, UFO_DOWN_LEFT_SPRITE, UFO_RIGHT_SPRITE,
            UFO_UP_RIGHT_SPRITE, UFO_DOWN_RIGHT_SPRITE, UFO_UP_SPRITE, UFO_DOWN_SPRITE, UFO_IDLE_SPRITE;

        public Ufo(Image ufoSprite, Canvas ufoCanvas)
        {
            SPRITE = ufoSprite;
            CANVAS = ufoCanvas;
            SPEED = 400;
            HP = 3;
            BULLET = 1;
            BULLETCOOLDOWN = 1.0;
            UFO_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left.png";
            UFO_UP_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left-up.png";
            UFO_DOWN_LEFT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-left-down.png";
            UFO_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right.png";
            UFO_UP_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right-up.png";
            UFO_DOWN_RIGHT_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-right-down.png";
            UFO_UP_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-up.png";
            UFO_DOWN_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\ufo-down.png";
            UFO_IDLE_SPRITE = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\IMG\\logo.png";
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
        }
    }

    public partial class MainWindow : Window
    {
        private SoundPlayer bgMusicSFX = new SoundPlayer(Properties.Resources.background);
        private MediaPlayer alienClickSFX = new MediaPlayer();
        private MediaPlayer uiClickSFX = new MediaPlayer();
       
        private DispatcherTimer timer = null;
        private DateTime lastUpdateTime;
        
        private Ufo UFO;
        private InputAxis INPUTS;

        private const string URL = "https://github.com/snezh0ks/VTORJENIE_NLO";
        private bool START;
        private double directionX = 0, directionY = 0;

        public MainWindow()
        {
            InitializeComponent();
            bgMusicPlay();
            alienAnimationPlay();

            UFO = new Ufo(ufoSprite, ufoCanvas);
            INPUTS = new InputAxis();
            lastUpdateTime = DateTime.Now;
            START = false;
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
            START = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // основной цикл игры

            Console.WriteLine(directionX);
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
        }

        private async void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            // кнопка запуска игры

            UiClickPlay();
            await Task.Delay(500);
            MENU.Visibility = Visibility.Hidden;
            GAME.Visibility = Visibility.Visible;
            timerStart();
            bgAnimationPlay();
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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // обработка движения влево-вправо при нажатии клавиши
            
            if (!START) return;

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

            if (!START) return;

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
        }
    }
}
