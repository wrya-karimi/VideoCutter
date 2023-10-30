using FFmpeg.NET;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoCutter
{
    public class CutVideo
    {
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public async void ClipVideo(SubTitle subTitle, string outputPath)
        {
            try
            {
                var fileDirectory = Path.GetDirectoryName(subTitle.FileAddress);

                MediaFile inputFile = null;
                if (Directory.GetFiles(fileDirectory, $"{GetName(subTitle.FileAddress)}.mkv").Count() > 0)
                {
                    inputFile = new MediaFile(subTitle.FileAddress.Replace("srt", "mkv"));
                }
                else if (Directory.GetFiles(fileDirectory, $"{GetName(subTitle.FileAddress)}.avi").Count() > 0)
                {
                    inputFile = new MediaFile(subTitle.FileAddress.Replace("srt", "avi"));
                }
                else if (Directory.GetFiles(fileDirectory, $"{GetName(subTitle.FileAddress)}.mp4").Count() > 0)
                {
                    inputFile = new MediaFile(subTitle.FileAddress.Replace("srt", "mp4"));
                }


                if (inputFile != null)
                {
                    var enginePath = ConfigurationManager.AppSettings.Get("EnginePath");
                    var ffmpeg = new Engine($"{enginePath}\\ffmpeg.exe");
                    var options = new ConversionOptions();
                    Regex r = new Regex(@"(\d\d):(\d\d):(\d\d),(\d\d\d)");

                    var start = r.Match(subTitle.Time);
                    var end = r.Match(subTitle.Time).NextMatch();

                    options.CutMedia(TimeSpan.FromSeconds(TimeSpan.Parse(start.Value.Replace(',', '.')).TotalSeconds - 13), TimeSpan.FromSeconds(25));

                    var outputFile = new MediaFile(outputPath + GetName(subTitle.FileAddress) + ".mp4");
                    if (Directory.GetFiles(outputPath, $"{GetName(subTitle.FileAddress)}.mp4").Count() > 0)
                    {
                        outputFile = new MediaFile(outputPath + GetName(subTitle.FileAddress) + DateTime.UtcNow.Ticks + ".mp4");
                    }
                    await ffmpeg.ConvertAsync(inputFile, outputFile, options);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static string GetName(string path)
        {
            var pathParts = path.Split('\\');
            string pathLastPart = pathParts[pathParts.Length - 1];
            return pathLastPart.Substring(0, pathLastPart.Length - 4);
        }
    }
}
