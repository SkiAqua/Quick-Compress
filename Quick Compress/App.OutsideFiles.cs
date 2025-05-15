using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace Quick_Compress
{
    public partial class App : Application
    {
        private void ValidateFilePath(string VideoPath) // End the program if the file dont exists
        {
            if (String.IsNullOrEmpty(VideoPath))
            {
                Environment.Exit(0);
            }
        }
        protected string GetVideoPath() // Get VideoPath by Win32.OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Selecione um arquivo de vídeo";
            dialog.Filter = "Arquivos de Vídeo|*.mp4;*.mkv;*.avi;*.mov;*.webm;*.flv;*.wmv;*.mpeg;*.mpg";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }
        private bool CheckAndPromptTool()
        {
            if (File.Exists(ReadonlyToolPath))
            {
                return true;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("O FFMPEG não foi encontrado, procurar diretório?", "ffmpeg not found", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    return true;
                }

                return false;
            }
        }
    }
}
