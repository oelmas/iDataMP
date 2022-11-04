using System.Collections.Generic;
using Newtonsoft.Json;

namespace AltitudeAngelWings.ApiClient.Models
{
    public class ApprovalOption
    {
        [JsonProperty("sections")] public List<ApprovalSection> Sections { get; set; }

        [JsonProperty("conditions")] public List<Condition> Conditions { get; set; } = new List<Condition>();
    }
}