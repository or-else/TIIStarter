using EBBuildClient.Core;
using EBBuildClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public class brains:CommonInterfaces
    {

        [JsonPropertyName("brain_id")]
        public Guid brain_id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("model")]
        public string model { get; set; }

        [JsonPropertyName("max_tokens")]
        public int max_tokens { get; set; }

        [JsonPropertyName("temperature")]
        public float temperature { get; set; }

        [JsonPropertyName("openai_api_key")]
        public string openai_api_key { get; set; }

        [JsonPropertyName("FKprompt_id")]
        public Guid FKprompt_id { get; set; }



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
