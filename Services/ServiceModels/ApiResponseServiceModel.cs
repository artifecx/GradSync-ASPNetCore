using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Services.ServiceModels
{
    public class ApiResponseServiceModel
    {
        [JsonPropertyName("match_percentages")]
        public Dictionary<string, double> MatchPercentages { get; set; }
    }
}
