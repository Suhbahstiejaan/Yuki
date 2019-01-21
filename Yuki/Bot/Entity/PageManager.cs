using System;
using System.Collections.Generic;
using Yuki.Bot.Common;

namespace Yuki.Bot.Entity
{
    public class PageManager
    {
        public List<Page> pages = new List<Page>();

        private int maxPerPage = 20;
        private string dataType;

        public PageManager(string[] data, string dataType)
        {
            this.dataType = dataType;

            /*
             * oh boy page creation :o
             * 
             * Note: I wrote this a while ago, came back to comment it,
             * and am completely confused on what exactly some of this does. 
             * 
             * I can't follow math.
             */
            if (data != null)
            {
                try
                {
                    for (int i = 0; i < data.Length; i += maxPerPage)
                    {
                        int pos = i + maxPerPage;
                        int total = 0;

                        if (data.Length - pos < 0)
                            pos = i + (data.Length - i);

                        /* set the amount of commands shown on the page */
                        total = pos - i;

                        Page page = new Page()
                        {
                            DataOnPage = pos
                        };

                        int div10 = 10;

                        /* Create page elements */
                        for (int j = i; j < pos; j++)
                        {
                            if (j % 10 == 0 && j != 0)
                                div10 = j;
                            //calculate the amount of spaces
                            int amtSpaces = Math.Abs(Math.Abs(total - j) - 2) / div10;
                            string spaces = new String(' ', amtSpaces);
                            page.Value += "[" + (j + 1) + "]" + spaces + "\t" + data[j] + "\n";
                        }

                        pages.Add(page);
                    }
                }
                catch(Exception e) { Logger.Instance.Write(LogLevel.Error, e); }

                Page[] _pages = pages.ToArray();

                for (int i = 0; i < _pages.Length; i++)
                {
                    string showing = "Showing " + _pages[i].DataOnPage + "/" + data.Length + " " + dataType + "(s)";
                    _pages[i].Value = "```\nPage " + (i + 1) + "\n\n" + _pages[i].Value + "\n" + new String('-', showing.Length) + "\n" + showing + "```";
                }
            }
        }

        public string GetPage(int pageNum)
        {
            if (pages.Count == 0)
                return "Could not find any " + dataType + "s";
            else if (pageNum > pages.Count)
                return pages[pages.Count - 1].Value;
            else if (pageNum < 0)
                return pages[0].Value;

            return pages[pageNum - 1].Value;
        }
    }
}
