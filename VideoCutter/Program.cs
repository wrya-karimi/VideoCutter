using FFmpeg.NET;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class Program
    {
        const string format = @"hh\:mm\:ss\,fff";
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter a text:");
            string key = Console.ReadLine();
            Console.WriteLine($"Start searching for \"{key}\"...");
            List<SubTitle> lstSubtitle = new List<SubTitle>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType.ToString() == "Fixed" /*&& drive.Name != "C:\\"*/)
                {
                    string[] dirs = Directory.GetDirectories(drive.Name, "*.*", SearchOption.TopDirectoryOnly);

                    foreach (var dir in dirs)
                    {
                        if (!dir.Contains("System Volume Information") /*&& !(drive.Name == "C:\\" && dir.Contains("Documents and Settings"))*/)
                        {
                            try
                            {
                                string[] filePaths = Directory.GetFiles(dir, "*.srt", SearchOption.AllDirectories);
                                foreach (var file in filePaths)
                                {
                                    string[] lines = File.ReadAllLines(file);

                                    var subs = new List<KeyValuePair<string, string>>();
                                    string time = "";
                                    string dialoge = "";
                                    int counter = 1;

                                    foreach (var line in lines)
                                    {
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            if (counter == 1)
                                            {
                                                counter++;
                                            }
                                            else if (counter == 2)
                                            {
                                                time = line;
                                                counter++;
                                            }
                                            else
                                            {
                                                dialoge += line + " ";
                                            }
                                        }
                                        else
                                        {
                                            subs.Add(new KeyValuePair<string, string>(time, dialoge));
                                            counter = 1;
                                            time = "";
                                            dialoge = "";
                                        }
                                    }

                                    for (int i = 0; i < subs.Count; i++)
                                    {
                                        if (subs[i].Value.ToLower().Contains(key.ToLower()))
                                        {
                                            var subtitle = new SubTitle();

                                            subtitle.Time = subs[i].Key;
                                            subtitle.Title = subtitle.BuildDialoge(i, subs);

                                            subtitle.FileAddress = file;

                                            lstSubtitle.Add(subtitle);
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }


            //after find the text it's turn to Extract the video
            if (lstSubtitle.Count > 0)
            {
                var file = new FileManager();
                List<string> textsList = new List<string>();
                string path = key.Replace(",", "").Replace("?", "").Replace("!", "").Replace(".", "").Replace("'", "").Trim();
                int secondsOfSleep = int.Parse(ConfigurationManager.AppSettings.Get("SecondsOfSleepAfterEachCut"));
                string outputDirectory = ConfigurationManager.AppSettings.Get("OutputDirectory");
                string outputPath = $"{outputDirectory}\\{path}\\";
                file.MakeDirectory(outputPath);
                Console.WriteLine($"{lstSubtitle.Count} items found");
                Console.WriteLine("Start Cutting videos...");
                for (int i = 0; i < lstSubtitle.Count; i++)
                {
                    var item = lstSubtitle[i];
                    Console.WriteLine($"#{i+1}");
                    //if (item.Title.Split(' ').Where(x => x.ToLower().Equals(key.ToLower())).Count() > 0)
                    {
                        (new CutVideo()).ClipVideo(item, outputPath);
                        textsList.Add(item.FileAddress);
                        textsList.Add(item.Time);
                        textsList.Add(item.Title);
                        textsList.Add("* * * * * * * *");
                        Thread.Sleep(secondsOfSleep * 1000);
                    }
                }
                file.SaveText(textsList, $"{outputPath}\\{path}.txt");
            }

            Console.WriteLine("Finish");
        }
    }
}
