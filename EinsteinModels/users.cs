using EBBuildClient.Core;
using EBBuildClient.Core.Interfaces;
using System.Text.Json.Serialization;

namespace EinsteinModels
{
    public class users : CommonInterfaces
    {

        [JsonPropertyName("user_id")]
        public Guid user_id { get; set; }

        [JsonPropertyName("FKuser_id")]
        public Guid FKuser_id { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("date")]
        public DateTime date { get; set; }

        [JsonPropertyName("requests_count")]
        public int requests_count { get; set; }



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