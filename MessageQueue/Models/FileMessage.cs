namespace MessageQueue.Models
{
    public class FileMessage
    {
        public string OriginLocation { get; set; }
        public string NewLocation { get; set; }
        public string FileName { get; set; }
    }
}
