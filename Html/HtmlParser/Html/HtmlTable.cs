using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Domain
{
    public class HtmlTable
    {
        public HtmlTable() { }
        public HtmlRow Headers { get; set; } = new();
        public List<HtmlRow> Items { get; set; } = new();
    }
}
