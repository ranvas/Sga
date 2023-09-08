using HtmlAgilityPack;
using HtmlParser.Domain;
using HtmlParser.Html;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace HtmlParser
{
    public class HtmlAdapter<TypeHtml> where TypeHtml: class
    {
        private SluchajHtmlClient _htmlClient;
        public HtmlAdapter(SluchajHtmlClient htmlСlient)
        {
            _htmlClient = htmlСlient;
        }

        public async Task<TModel> ImportExampleAsync<TModel, TDomainManager>(TDomainManager manager) where TDomainManager : DomainManager where TModel : class
        {
            if(typeof(HtmlTable).IsAssignableFrom(typeof(TypeHtml)))
            {
                var table = await _htmlClient.ImportTableAsync();
                
                var result = await manager.ImportAsync<TModel, HtmlTable>(table);
                return result;
            }
            throw new NotImplementedException();
        }
    }
}