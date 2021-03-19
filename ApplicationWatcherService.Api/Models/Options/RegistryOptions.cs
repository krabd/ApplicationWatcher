namespace ApplicationWatcherService.Api.Models.Options
{
    public class RegistryOptions
    {
        public const string Registry = "Registry";

        public string BasePath { get; set; }

        public string ExeValue { get; set; }

        public string LogsValue { get; set; }
    }
}
