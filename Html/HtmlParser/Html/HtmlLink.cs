using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Domain
{
    public class HtmlLink
    {
        public virtual string Url { get; set; } = string.Empty;
        public virtual string Name { get; set; } = string.Empty;
    }
}
