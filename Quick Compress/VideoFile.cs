using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows;

namespace Quick_Compress
{
    public class VideoInfo
    {
        public bool IsInitiated = false;

        public string VideoPath;
        public long VideoBitRate;
        
        public VideoInfo(string videoPath) 
        {
            InitInfoCollect(videoPath);
        }

        public VideoInfo()
        {

        }

        public void InitInfoCollect(string videoPath)
        {
            if (IsInitiated)
            {
                throw new Exception("Already initiated!");
            }
            
            VideoPath = videoPath;


            using JsonDocument doc = GetProcessInfo();

            SaveDocumentInfo(doc);

        }

        private JsonDocument GetProcessInfo()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = App.ReadonlyToolPath,
                Arguments = $"-v quiet -print_format json -show_format \"{VideoPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return JsonDocument.Parse(output);
        }

        private void SaveDocumentInfo(JsonDocument doc)
        {
            var root = doc.RootElement;

            if (root.TryGetProperty("format", out JsonElement format) &&
                format.TryGetProperty("bit_rate", out JsonElement bitrateElement))
            {
                VideoBitRate = long.Parse(bitrateElement.GetString());
            }
            else
            {
                MessageBox.Show("Bitrate não encontrado");
            }
        }
    }
}
