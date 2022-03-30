using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SubTitle
    {
        public string Title { get; set; }
        public string Time { get; set; }
        public string FileAddress { get; set; }


        public string BuildDialoge(int position, List<KeyValuePair<string, string>> subs)
        {
            string dialoge = "";
            int total = subs.Count;
            int begin = position - 5;
            int end = position + 5;

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

            return dialoge;
        }
    }
}
