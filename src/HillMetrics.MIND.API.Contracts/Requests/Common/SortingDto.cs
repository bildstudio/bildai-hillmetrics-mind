using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Contracts.Requests.Common
{
    public class SortingDto
    {
        /// <summary>
        /// The field to sort by
        /// </summary>
        [FromQuery(Name = "field")]
        public string Field { get; set; }

        /// <summary>
        /// The direction to sort by
        /// </summary>
        [FromQuery(Name = "direction")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SortDirection Direction { get; set; }
    }
}
