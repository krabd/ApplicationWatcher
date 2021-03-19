namespace ApplicationWatcher.Service.Api.Models.Options
{
    public class HealthCheckOptions
    {
        public const string HealthCheck = "HealthCheck";

        public int TimeoutSeconds { get; set; }
    }
}
