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

        private SoundPlayer bg_music = new SoundPlayer(Properties.Resources.soundBG);

        public MainWindow()
        {
            InitializeComponent();
            PlayBackgroundMusic();
            StartVerticalAnimation();
        }

        private void PlayBackgroundMusic()
        {
            bg_music.PlayLooping();
        }

        private void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void StartVerticalAnimation()
        {
            DoubleAnimation verticalAnimation = new DoubleAnimation
            {
                From = 0,        
                To = 20,
                Duration = new Duration(TimeSpan.FromSeconds(3.0)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            ufoTransform.BeginAnimation(TranslateTransform.YProperty, verticalAnimation);
        }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

        private void OpenCodeButtonClick(object sender, RoutedEventArgs e)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\TEXT\\codeTemp.txt";

            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\TEXT\\code.txt", directoryPath, true);

            Console.WriteLine(directoryPath);

            string code = Properties.Resources.code;

            Process.Start("notepad.exe", directoryPath);

            string url = "https://github.com/snezh0ks/VTORJENIE_NLO";

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });


        }
    }
}
