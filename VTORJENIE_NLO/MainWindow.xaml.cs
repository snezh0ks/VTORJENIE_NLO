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

namespace VTORJENIE_NLO
{

    public partial class MainWindow : Window
    {

        private SoundPlayer backgroundMusic = new SoundPlayer(Properties.Resources.background);
        private MediaPlayer voiceSound = new MediaPlayer();
        private MediaPlayer uiSound = new MediaPlayer();
        private string url = "https://github.com/snezh0ks/VTORJENIE_NLO";

        public MainWindow()
        {
            InitializeComponent();
            PlayBackgroundMusic();
            logoAnimation();
        }

        private void PlayBackgroundMusic()
        {
            backgroundMusic.PlayLooping();
        }

        private async void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            UiClick();

            await Task.Delay(1000);

            Environment.Exit(0);
        }

        private void logoAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation { From = 0, To = 20, 
                Duration = new Duration(TimeSpan.FromSeconds(3.0)), AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever };
            logoTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void OpenCodeButtonClick(object sender, RoutedEventArgs e)
        {
            UiClick();
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void Voice(object sender, RoutedEventArgs e)
        {
            voiceSound.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\voice.wav"));
            voiceSound.Play();
        }

        private void UiClick()
        {
            uiSound.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\Resources\\ui.wav"));
            uiSound.Play();
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            UiClick();
        }
    }
}
