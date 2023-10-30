using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCutter
{
    public class SubTitle
    {
        private Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Title { get; set; }
        public string Time { get; set; }
        public string FileAddress { get; set; }

        public string BuildDialoge(int position, List<KeyValuePair<string, string>> subs)
        {
            string dialoge = "";
            int begin = position - 5;
            int end = position + 5;

            try
            {
                if (begin >= 0)
                {
                    for (int i = begin; i < subs.Count; i++)
                    {
                        dialoge += subs[i].Value + Environment.NewLine;
                        if (i == end)
                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < subs.Count; i++)
                    {
                        dialoge += subs[i].Value + Environment.NewLine;
                        if (i == end)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return dialoge;
        }
    }
}
