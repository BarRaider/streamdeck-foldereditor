using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_foldereditor.Models
{
    internal class PageInfo
    {
        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public Dictionary<string, PageAction> Actions { get; set; }
    }
}
