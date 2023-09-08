using HtmlParser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Domain
{
    public class HtmlRow
    {
        public virtual List<HtmlCell> Cells { get; set; } = new();
        public string GetString(string name, HtmlRow headers)
        {
            var index = GetIndex(name, headers);
            if (index == -1)
                throw new Exception($"{name} not found");
            return Cells[index].Text;
        }

        public string GetHtml(string name, HtmlRow headers)
        {
            var index = GetIndex(name, headers);
            return Cells[index].Html;
        }

        public int GetInt(string name, HtmlRow headers)
        {
            var index = GetIndex(name, headers);
            return int.Parse(Cells[index].Text);
        }
        private int GetIndex(string name, HtmlRow headers)
        {
            return headers.Cells.Select(c => c.Text).ToList().IndexOf(name);
        }
    }
}
