namespace Core.Processing
{
    public enum EventType
    {
        Info,
        Error,
        Warning
    }

    public class Event
    {
        public string Desctiption { get; set; }
        public string FileName { get; set; }
        public string ModName { get; set; }
        public EventType Type { get; set; }
    }
}