using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Quick_Compress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /*
     * Birate * * *
     * Resolução * *
     * Codec * * *
     * Frame rate *
     * Codec (Audio) *
     * Audio Channels *
     * Duração * * *
     */

    public partial class MainWindow : Window
    {
        App CurrentApplication = (App) Application.Current;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Botao_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("slk");
        }

        public void SetVideo(string videoDirectory)
        {
            long bitRateInKiloBytes = CurrentApplication.VideoFile.VideoBitRate / 1000;

            Video_Name.Content = $"Video: {videoDirectory}";
            Bit_Rate_Text.Content = $"{bitRateInKiloBytes.ToString("N0")} kbps";

        }

        public ProcessStartInfo GetVideoData(string videoDirectory)
        {
            if (!File.Exists(videoDirectory))
            {
                ;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {

            };

            return startInfo;
        }
    }
}