using EBBuildClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public class prompts: CommonInterfaces
    {
        [JsonPropertyName("id")]
        public Guid id { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        public prompts()
        {
            this.status = "private";
        }


        [JsonPropertyName("BlockName")]
        public string BlockName { get; set; }


        [JsonPropertyName("BlockHashCode")]
        public string BlockHashCode { get; set; }


        [JsonPropertyName("FuzzyMatchRatios")]
        public string FuzzyMatchRatios { get; set; }

        [JsonPropertyName("GroupCount")]
        public string GroupCount { get; set; }


        [JsonPropertyName("ChildRelationships")]
        public Dictionary<string, List<dynamic>> ChildRelationships { get; set; }



    }
}
