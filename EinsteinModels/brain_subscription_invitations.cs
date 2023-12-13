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
    public class brain_subscription_invitations:CommonInterfaces
    {

        [JsonPropertyName("brain_id")]
        public Guid brain_id { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("rights")]
        public string rights { get; set; }

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
