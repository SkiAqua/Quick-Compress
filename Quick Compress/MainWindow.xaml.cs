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
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace Quick_Compress
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		App CurrentApplication = (App) Application.Current;
		VideoInfo VideoFile;

        private static string[] _resolutionsArray =
		{
			"8K",
			"4K",
			"2K (QHD)",
			"1080p",
			"900p",
			"720p",
			"480p",
			"360p",
			"240p",
			"144p"
        };

        public MainWindow(VideoInfo vf)
		{
			InitializeComponent();
			SetResolutionsComboBox();

			VideoFile = vf;
		}
		public void SetResolutionsComboBox()
		{
			bool chkd = false;

            foreach (string resolutionName in _resolutionsArray)
			{
				if (resolutionName == CurrentApplication.VideoFile.ResolutionName)
					chkd = true;
				if (chkd)
					txtRelolution_Selector.Items.Add(resolutionName);
			}
            txtRelolution_Selector.SelectedIndex = 0;
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

			txtFramerate_Selector.Text = videoInfo.FrameRate.ToString();
			txtFramerate_Selector.ToolTip = $"Default: {videoInfo.FrameRate} ({videoInfo.RawFrameRate})";

			if (videoInfo.Size < 1_000_000)
			{
                FileSizeSelector.Text = Math.Round((double) videoInfo.Size / 1000f, 2).ToString();
				MeasureSizeSelector.SelectedIndex = 2;
            }
			else if (videoInfo.Size < 1_000_000_000)
			{
				FileSizeSelector.Text = Math.Round((double) videoInfo.Size / 1000000f, 2).ToString();
				MeasureSizeSelector.SelectedIndex = 1;
			}
			else if (videoInfo.Size < 1_000_000_000_000)
			{
				FileSizeSelector.Text = Math.Round((double) videoInfo.Size / 1000000000f, 2).ToString();
				MeasureSizeSelector.SelectedIndex = 0;
			}
			FileSizeSelector.ToolTip = $"Default: {FileSizeSelector.Text} {((ComboBoxItem) (MeasureSizeSelector.SelectedItem)).Content}";
		}
		public async void Compress(object sender, RoutedEventArgs e)
		{

			SaveFileDialog saveDialog = new SaveFileDialog();

			saveDialog.Filter = "Arquivos de Vídeo|*.mp4;*.mkv;*.avi;*.mov|Todos os Arquivos|*.*";
			saveDialog.Title = "Salvar Vídeo";
			saveDialog.DefaultExt = ".mp4";
			saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(VideoFile.VideoPath) + " compressed" + System.IO.Path.GetExtension(VideoFile.VideoPath);

			string fileName = saveDialog.FileName;
			string codecName = VideoFile.CodecName;
			double frameRate = Double.Parse(txtFramerate_Selector.Text);

            bool? alright = saveDialog.ShowDialog();

            if (alright == true)
			{
				CompressBar_Canvas.Visibility = Visibility.Visible;

                await Task.Delay(100);

                ulong fileSize = MeasureSizeSelector.Text switch
				{
					"KB" => UInt32.Parse(FileSizeSelector.Text) * 1_000,
					"MB" => UInt32.Parse(FileSizeSelector.Text) * 1_000_000,
					"GB" => UInt32.Parse(FileSizeSelector.Text) * 1_000_000_000
				};

				await Task.Run(() =>
				{
					VideoFile.Compress(saveDialog.FileName, VideoFile.CodecName, frameRate, fileSize, "1.0");
				});
				CompressBar_Canvas.Visibility = Visibility.Hidden;
			}
        }
		public void IntOnly(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !int.TryParse(e.Text, out _);
		}
	}
}