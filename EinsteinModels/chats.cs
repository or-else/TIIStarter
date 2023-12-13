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
    public class chats:CommonInterfaces
    {

        [JsonPropertyName("chat_id")]
        public Guid chat_id { get; set; }

        [JsonPropertyName("FKuser_id")]
        public Guid FKuser_id { get; set; }

        [JsonPropertyName("creation_time")]
        public DateTime creation_time { get; set; }

        [JsonPropertyName("history")]
        public string history { get; set; }

        [JsonPropertyName("chat_name")]
        public string chat_name { get; set; }




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
