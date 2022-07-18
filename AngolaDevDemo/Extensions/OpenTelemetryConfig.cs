using System.Diagnostics;

namespace AngolaDevDemo.Extensions
{
    public static class OpenTelemetryConfig
    {
        public static string Local { get; }
        public static string ServiceName { get; }
        public static string ServiceVersion { get; }

        static OpenTelemetryConfig()
        {
            Local = "AngolaDevSummit";
            ServiceName = typeof(OpenTelemetryConfig).Assembly.GetName().Name!;
            ServiceVersion = typeof(OpenTelemetryConfig).Assembly.GetName().Version!.ToString();
        }

        public static ActivitySource CreateActivitySource() =>
            new ActivitySource(ServiceName, ServiceVersion);
    }
}
