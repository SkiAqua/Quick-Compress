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

        public void SetVideo(VideoInfo videoInfo)
        {
            uint bitRateInKiloBytes = (uint) (CurrentApplication.VideoFile.Bitrate / 1000);

            if (videoInfo.Title != null)
                txtVideo_Title.Content = videoInfo.Title;
            else
                txtVideo_Title.Visibility = Visibility.Collapsed;
            
            txtVideo_Name.Content = $"Video: {videoInfo.VideoPath}";
            
            txtBit_Rate_Text.Content = $"{bitRateInKiloBytes.ToString("N0")} kbps";
            txtBit_Rate_Text.ToolTip = $"Default: {bitRateInKiloBytes.ToString("N0")} kbps";

            txtCodecLabel.Content = $"Codec: {videoInfo.CodecName}";
            txtCodecLabel.ToolTip = $"Default: {videoInfo.CodecName}";

        }
        public void IntOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}