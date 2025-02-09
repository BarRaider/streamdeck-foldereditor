using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace streamdeck_foldereditor.Models
{
    internal class ProfileInfo
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public PagesOverview Pages { get; set; }

        [JsonIgnore]
        public string FullPath { get; set; }

        [JsonProperty]
        public string Version { get; set; }

        [JsonProperty]
        public Device Device { get; set; }
    }
}
