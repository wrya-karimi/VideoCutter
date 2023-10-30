using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCutter
{
    public class FileManager
    {
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void MakeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void SaveText(List<string> text, string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        foreach (var txt in text)
                        {
                            sw.WriteLine(txt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
