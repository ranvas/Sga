using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBot.DataSphere
{
    public class RootParams
    {
        public RootParams() { }
        public RootParams(string arams) 
            : this()
        {
            Source = arams;
        }
        public string Source { get; set; } = string.Empty;

        public List<long> Roots { get; set; } = new();

        public List<long> Logs { get; set; } = new();

        public string Secret { get; set; } = "terces";
    }
}
