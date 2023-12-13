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
    public class chat_history:CommonInterfaces
    {
        [JsonPropertyName("message_id")]
        public Guid message_id { get; set; }

        [JsonPropertyName("FKchat_id")]
        public Guid FKchat_id { get; set; }

        [JsonPropertyName("user_message")]
        public string user_message { get; set; }

        [JsonPropertyName("assistant")]
        public string assistant { get; set; }

        [JsonPropertyName("message_time")]
        public DateTime message_time { get; set; }

        [JsonPropertyName("FKprompt_id")]
        public Guid FKprompt_id { get; set; }

        [JsonPropertyName("FKbrain_id")]
        public Guid FKbrain_id { get; set; }



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
