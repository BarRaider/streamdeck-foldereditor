using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamdeck_foldereditor.Models
{
    internal class Device
    {
        [JsonProperty]
        public string Model { get; set; }

        [JsonProperty]
        public string UUID { get; set; }
    }
}
