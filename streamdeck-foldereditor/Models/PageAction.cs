using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_foldereditor.Models
{
    internal class PageAction
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string UUID { get; set; }

        [JsonProperty]
        public int State { get; set; }

        [JsonProperty]
        public JObject Settings { get; set; }

        [JsonProperty]
        public JObject[] States { get; set; }
    }
}
