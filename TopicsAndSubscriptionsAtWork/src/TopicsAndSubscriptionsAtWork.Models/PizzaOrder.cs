using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TopicsAndSubscriptionsAtWork.Models
{
    public class PizzaOrder
    {
        [JsonProperty()]
        public string Name { get; set; }
        [JsonProperty()]
        public string Size { get; set; }
        [JsonProperty()]
        public bool HasMeat { get; set; }
    }
}
