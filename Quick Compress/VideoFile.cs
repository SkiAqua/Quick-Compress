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

// size
// duration
// format_name
// probe_score
// tags title
namespace Quick_Compress
{
    public class VideoInfo
    {
        public bool IsInitiated = false;

        public string VideoPath;

        public ulong Bitrate;
        public uint Width;
        public uint Height;
        public string? ResolutionName;
        public ulong Size;
        public double Duration;

        public string? CodecName;
        public string? FormatName;
        public int ProbeScore;
        public string? Title;

        public VideoInfo(string videoPath)
        {
            //
            VideoPath = videoPath;
            InitInfoCollect(videoPath);
        }

        private void InitInfoCollect(string videoPath)
        {
            // A VideoInfo can't initialize twice.
            if (IsInitiated)
                throw new Exception("Already initiated!");

            // Gather information from CMD
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = App.ReadonlyToolPath,
                Arguments = $"-v quiet -print_format json -show_format -show_streams \"{VideoPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            using JsonDocument doc = JsonDocument.Parse(output);
            //
            var root = doc.RootElement;

            // Gather information of "format"
            if (root.TryGetProperty("format", out JsonElement format))
            {
                // Bitrate
                if (format.TryGetProperty("bit_rate", out JsonElement bitrateElement) && !String.IsNullOrEmpty(bitrateElement.GetString()))
                    Bitrate = ulong.Parse(bitrateElement.GetString());
                else
                    MessageBox.Show("Bitrate não encontrado");

                // Size
                if (format.TryGetProperty("size", out JsonElement sizeElement) && !String.IsNullOrEmpty(sizeElement.GetString()))
                    Size = ulong.Parse(sizeElement.GetString());
                else
                    MessageBox.Show("Tamanho não encontrado");

                // Duration
                if (format.TryGetProperty("duration", out JsonElement durationElement) && !String.IsNullOrEmpty(durationElement.GetString()))
                    Duration = double.Parse(durationElement.GetString());
                else
                    MessageBox.Show("Duração não foi encontrado");
                
                // Title
                if (format.TryGetProperty("tags", out JsonElement tags) && tags.TryGetProperty("Title", out JsonElement title)) 
                    Title = title.GetString();
            }
            // Gather information of "streams"
            JsonElement videoStream = default;

            if (root.TryGetProperty("streams", out JsonElement streams))
            {
                // Verify if the vídeo have more than 1 video stream
                int streamCount = 0;
                foreach (JsonElement stream in streams.EnumerateArray())
                {
                    if (stream.TryGetProperty("codec_type", out JsonElement value))
                        if (value.GetString().Equals("video"))
                        { 
                            streamCount++;
                            if (streamCount == 1)
                                videoStream = stream;
                        }
                    
                }

                if (streamCount < 1)
                {
                    MessageBox.Show("Não foi encontrado nenhum vídeo nesse arquivo.");
                    Environment.Exit(0);
                } else if (streamCount > 1)
                {
                    MessageBox.Show("Esse programa atualmente não suporta compressão de arquivo com mais de um vídeo.");
                    Environment.Exit(0);
                }
                     

                // CodecName
                if (videoStream.TryGetProperty("codec_name", out JsonElement codecNameElement) && !String.IsNullOrEmpty(codecNameElement.GetString()))
                    CodecName = codecNameElement.GetString();
                else
                    MessageBox.Show("Codec não encontrado");

                // Resolution
                if (videoStream.TryGetProperty("width", out JsonElement widthElement) && videoStream.TryGetProperty("height", out JsonElement heightElement))
                {
                    Width = widthElement.GetUInt32();
                    Height = heightElement.GetUInt32();
                    ResolutionName = GetResolutionName(Width, Height);
                }
            }

        }

        public string GetResolutionName(uint width, uint height)
        {
            uint minr = Math.Min(width, height);

            if (minr <= 144)
                return "144p";
            else if (minr <= 240)
                return "240p";
            else if (minr <= 360)
                return "360p";
            else if (minr <= 480)
                return "480p";
            else if (minr <= 720)
                return "720p";
            else if (minr <= 900)
                return "900p";
            else if (minr <= 1080)
                return "1080p";
            else if (minr <= 1440)
                return "2K (QHD)";
            else if (minr <= 2160)
                return ("4K");
            else
                return "8K";
        }
    }
}
