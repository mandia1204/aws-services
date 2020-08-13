using System;

namespace Models
{
    public class CloudTrailS3Event
    {
        public string SourceIPAddress { get; set; }
        public string UserAgent { get; set; }
        public string EventType { get; set; }
        public string EventCategory { get; set; }
        public string RequestID { get; set; }
        public string EventName { get; set; }
        public DateTime EventTime { get; set; }
        public string EventSource { get; set; }
    }
}
