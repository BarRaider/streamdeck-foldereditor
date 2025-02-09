using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamdeck_foldereditor.Models
{
    internal class Page
    {
        [JsonProperty]
        public List<PageInfo> Controllers { get; set; }

        [JsonIgnore]
        public string FullPath { get; set; }
    }
}
