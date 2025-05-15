using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows;
using System.Security.Cryptography;
using System.Security.Policy;

// size
// duration
// format_name
// probe_score
// tags title
namespace Quick_Compress
{
    public class VideoInfo
    {
        private byte[] Signature;

        public bool IsInitiated = false;

        public string VideoPath;

        public ulong Bitrate;

        public double FrameRate;
        public string? RawFrameRate;

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
            else if (!File.Exists(VideoPath))
            {
                MessageBox.Show("O vídeo já não existe.");
            }
            // Gather Hash
            Signature = GetHashFrom(videoPath);
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
                // FrameRate
                if (videoStream.TryGetProperty("avg_frame_rate", out JsonElement avgFrameRateElement) && !String.IsNullOrEmpty(avgFrameRateElement.GetString()))
                    RawFrameRate = avgFrameRateElement.GetString();

                if (Double.TryParse(RawFrameRate, out double newFrameRate))
                    FrameRate = newFrameRate;
                else
                {
                    string[] frParts = RawFrameRate.Split('/');

                    if (frParts.Length != 2)
                    {
                        MessageBox.Show("Não foi possível entender a frame rate.");
                        Environment.Exit(0);
                    }

                    FrameRate = Double.Parse(frParts[0]) / Double.Parse(frParts[1]);
                }
                FrameRate = Math.Round(FrameRate, 4);
            }

        }
        public static string GetBitRateQuality(VideoInfo vi, string codec)
        {
            MessageBox.Show("A função GetBitRateQuality ainda não está funcionando");
            /*
            if (String.IsNullOrEmpty(vi.CodecName))
            {
                MessageBox.Show("Não foi possível estimar a qualidade.");
                Environment.Exit(0);
            }
            double pixelPerSecond = vi.Width * vi.Height * vi.FrameRate;

            double qualityPoints = vi.Bitrate / pixelPerSecond;

            switch (codec)
            {
                case "foda":
                    break;
            }
            */
            return "";
        }
        public void Compress(string newPath, string newCodec, double newFrameRate, ulong newFileSize, string newResolutionName)
        {
            // Check path existence
            if (!File.Exists(VideoPath))
            {
                MessageBox.Show("O arquivo não foi encontrado.");
                Environment.Exit(0);
            }
            // Check file consistency
            if (!Signature.SequenceEqual(GetHashFrom(VideoPath)))
            {
                MessageBox.Show("O arquivo não é o mesmo!");
                Environment.Exit(0);
            }
            // Scale Manage
            uint[] newSize = GetScaledResolution(Width, Height, newResolutionName);

            // Calculate the bit rate
            ulong convertedBitRate = Bitrate * newFileSize / Size;
            // Start Process
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = App.ConversionToolPath,
                Arguments = $"-i \"{VideoPath}\" -c:v {newCodec} -r {newFrameRate} -b:v {convertedBitRate} -vf scale={newSize[0]}:{newSize[1]} -y \"{newPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(processStartInfo);
            string output = process.StandardError.ReadToEnd();
            process.WaitForExit();

            MessageBox.Show("Sucesso");
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
        public uint[] GetScaledResolution(uint inputWidth, uint inputHeight, string resolutionName) // returns {Width, Height}
        {
            bool landscape = inputWidth > inputHeight;

            uint outMin = GetResolutionByResolutionName(resolutionName);
            uint outMax = Math.Max(inputWidth, inputHeight) * outMin / Math.Min(inputWidth, inputHeight);

            return landscape ? new uint[] { outMax, outMin } : new uint[] { outMin, outMax };
        }
        private byte[] GetHashFrom(string videoPath)
        {
            using var sha = SHA256.Create();
            using var fileStream = File.OpenRead(videoPath);
            return sha.ComputeHash(fileStream);
        }
        private uint GetResolutionByResolutionName(string resolutionName)
        {
            uint result = resolutionName switch
            {
                "144p" => 144,
                "240p" => 240,
                "360p" => 360,
                "480p" => 480,
                "720p" => 720,
                "900p" => 900,
                "1080p" => 1080,
                "2K (QHD)" => 1440,
                "4K" => 2160,
                _ => 4320
            };

            return result;
        }

    }
}
