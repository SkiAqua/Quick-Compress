using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls;



namespace Quick_Compress
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string ConversionToolPath = @"C:\Users\S K I\Downloads\ffmpeg-7.0.2-full_build\ffmpeg-7.0.2-full_build\bin\ffmpeg.exe";
        public const string ReadonlyToolPath = @"C:\Users\S K I\Downloads\ffmpeg-7.0.2-full_build\ffmpeg-7.0.2-full_build\bin\ffprobe.exe";

        public VideoInfo VideoFile;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Search the Startup Args for the video path
            string VideoPath = GetStartupArgs(e);

            // If the video was not found, get by Win32.OpenFileDialog()
            if (String.IsNullOrEmpty(VideoPath))
            {
                VideoPath = GetVideoPath();

                ValidateFilePath(VideoPath); // Close if the user cancel the search
            }

            CheckAndPromptTool();

            VideoFile = new VideoInfo(VideoPath);

            MainWindow main_window = new MainWindow();

            main_window.SetVideo(VideoFile);
            main_window.Show();
        }

        public static void CreateErrorMessageBox(Exception ex) // Create generic Error Message
        {
            MessageBox.Show("Um erro do programa ocorreu:\n\n" + ex.ToString());
        }

        public string GetStartupArgs(StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                return "";
            } else {
                return e.Args[0];
            }
        }
    }

}
