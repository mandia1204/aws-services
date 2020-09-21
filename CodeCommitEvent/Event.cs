using System.Text.Json.Serialization;

namespace CodeCommitEvent
{
    public class Event
    {
        public string id { get; set; }
        [JsonPropertyName("detail-type")]
        public string detailType { get; set; }
        public string source { get; set; }
        public EventDetail detail { get; set; }
    }

    public  class EventDetail
    {
        [JsonPropertyName("event")]
        public string eventName { get; set; }
        public string repositoryId { get; set; }
        public string commitId { get; set; }
    }
}
