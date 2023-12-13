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
    public class brains_users:CommonInterfaces
    {
        [JsonPropertyName("brain_id")]
        public Guid brain_id { get; set; }

        [JsonPropertyName("user_id")]
        public Guid user_id { get; set; }

        [JsonPropertyName("rights")]
        public string rights { get; set; }

        [JsonPropertyName("default_brain")]
        public bool default_brain { get; set; }

        [JsonPropertyName("FKuser_id")]
        public Guid FKuser_id { get; set; }

        [JsonPropertyName("FKbrain_id")]
        public Guid FKbrain_id { get; set; }


        public brains_users()
        {
            this.default_brain = false;

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
