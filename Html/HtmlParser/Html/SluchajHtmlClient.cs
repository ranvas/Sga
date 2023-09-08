using HtmlAgilityPack;
using HtmlParser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Html
{
    public class SluchajHtmlClient
    {
        private HtmlDocument _htmlDocument;
        public SluchajHtmlClient(string inputHtmlText) 
        {
            _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(@$"{inputHtmlText}");
        }

        public Task<HtmlTable> ImportTableAsync()
        {
            var table = _htmlDocument.DocumentNode.SelectNodes("//table").FirstOrDefault() ?? throw new Exception("error gettable");
            var item = new HtmlTable();
            var collection = table.SelectNodes("tr");
            item.Headers = GetHeader(collection);
            item.Items = GetItems(collection);
            return Task.FromResult(item);
        }

        private List<HtmlRow> GetItems(HtmlNodeCollection collection)
        {
            var result = new List<HtmlRow>();
            foreach (var row in collection.Skip(1))
            {
                result.Add(GetRow(row, "td"));
            }
            return result;
        }

        private HtmlRow GetRow(HtmlNode? node, string name)
        {
            if (node == null)
                throw new Exception($"html node not found");
            var result = new HtmlRow();
            foreach (var cell in node.SelectNodes(name))
            {
                result.Cells.Add(new HtmlCell { Text = cell.InnerText, Html = cell.InnerHtml });
            }
            return result;
        }

        private HtmlRow GetHeader(HtmlNodeCollection collection)
        {
            var header = new HtmlRow();
            foreach (var row in collection.Take(1))
            {
                header = GetRow(row, "th");
            }
            return header;
        }
    }
}
