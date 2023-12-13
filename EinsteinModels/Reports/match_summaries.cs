using EBBuildClient.Core;
using EBBuildClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EinsteinModels.Reports
{
    public class match_summaries:CommonInterfaces
    {
        [JsonPropertyName("id")]
        public long id { get; set; }

        [JsonPropertyName("document_id")]
        public Guid document_id { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }

        [JsonPropertyName("metadata")]
        public string metadata { get; set; }

        [JsonPropertyName("embedding")]
        public Int32[,] embedding { get; set; }

        [JsonPropertyName("similarity")]
        public float similarity { get; set; }





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
