using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Domain
{
    public class DomainManager
    {
        protected DomainOptions Options { get; set; }
        public DomainManager() : this(new DomainOptions()) { }
        public DomainManager(DomainOptions options)
        {
            Options = options;
        }
        public virtual Task<TResult> ImportAsync<TResult, TModel>(TModel model) where TResult : class
        {
            throw new NotImplementedException();
        }
    }
}
