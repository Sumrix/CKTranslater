namespace Core.Processing
{
    public class FileContext
    {
        public string FullFileName { get; init; } = "";
        public string ModFolder { get; init; } = "";
        public ModuleInfo? ModInfo { get; init; }
    }
}