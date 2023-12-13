using EBBuildClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public class stats:CommonInterfaces
    {
        [JsonPropertyName("time")]
        public DateTime time { get; set; }

        [JsonPropertyName("chat")]
        public bool chat { get; set; }

        [JsonPropertyName("embedding")]
        public bool embedding { get; set; }

        [JsonPropertyName("details")]
        public string details { get; set; }
       
        [JsonPropertyName("metadata")]
        public string metadata { get; set; }

        [JsonPropertyName("id")]
        public long id { get; set; }



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
