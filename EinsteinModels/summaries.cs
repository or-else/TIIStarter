using EBBuildClient.Core;
using EBBuildClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public class summaries:CommonInterfaces
    {
        [JsonPropertyName("id")]
        public long id { get; set; }

        [JsonPropertyName("FKdocument_id")]
        public Guid FKdocument_id { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }

        [JsonPropertyName("metadata")]
        public string metadata { get; set; }

        [JsonPropertyName("embedding")]
        public string embedding { get; set; }  //Int32[,]


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
