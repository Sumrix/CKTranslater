namespace CKTranslater.Processing
{
    public enum EventType
    {
        Info,
        Error,
        Warning
    }

    public class Event
    {
        public string ModName { get; set; }
        public string FileName { get; set; }
        public EventType Type { get; set; }
        public string Desctiption { get; set; }
    }
}