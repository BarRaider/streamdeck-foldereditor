using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamdeck_foldereditor.Models
{
    internal class PagesOverview
    {
        [JsonProperty]
        public string Current { get; set; }

        [JsonProperty]
        public string Default { get; set; }

        [JsonProperty]
        public List<string> Pages { get; set; }
    }
}
