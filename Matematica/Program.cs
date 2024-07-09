using Azure.Monitor.OpenTelemetry.Exporter.Demo.Traces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Matematica
{
    class Program
    {
        static void Main(string[] args)
        {
            var request = new Request
            {
                Itens = new List<Item> {
                new Item
                {
                    Position = "001",  Code ="abc", Price = 120, Quantity = 1, Total = 120
                },
                new Item
                {
                    Position = "002",  Code ="xyz", Price = 234, Quantity = 1, Total = 234 }
                }
            };



            //TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            //configuration.ConnectionString = "InstrumentationKey=7366650a-0fa6-456b-9abf-5ae25056fb47;IngestionEndpoint=https://brazilsouth-1.in.applicationinsights.azure.com/;LiveEndpoint=https://brazilsouth.livediagnostics.monitor.azure.com/;ApplicationId=210c84aa-c019-419e-9fe4-0bebc350b3ff";
            //var telemetryClient = new TelemetryClient(configuration);
            //telemetryClient.TrackTrace("Hello World!");

            //using (InitializeDependencyTracking(configuration))
            //{
            // run app...

            using (var logDemo = new LogDemo("InstrumentationKey=7366650a-0fa6-456b-9abf-5ae25056fb47"))
            using (var traceDemo = new TraceDemo("InstrumentationKey=7366650a-0fa6-456b-9abf-5ae25056fb47"))
            using (var metricsDemo = new MetricDemo("InstrumentationKey=7366650a-0fa6-456b-9abf-5ae25056fb47"))
            {
                var logger = logDemo.loggerFactory.CreateLogger<TraceDemo>();
                 
                using (var trace = traceDemo.s_activitySource.StartActivity("ConsultaCampanha", System.Diagnostics.ActivityKind.Server))
                {
                    metricsDemo.GenerateMetrics();
                    new Fidelidade(logger).ConsultaCampanha(request, traceDemo.s_activitySource).GetAwaiter().GetResult();

                }
            }
            // traceDemo.GenerateTraces();

            //}

            //telemetryClient.Flush();

            //Task.Delay(5000).Wait();

            Console.ReadLine();
        }

        static DependencyTrackingTelemetryModule InitializeDependencyTracking(TelemetryConfiguration configuration)
        {
            var module = new DependencyTrackingTelemetryModule();

            // prevent Correlation Id to be sent to certain endpoints. You may add other domains as needed.
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.chinacloudapi.cn");
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.cloudapi.de");
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.usgovcloudapi.net");
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("localhost");
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("127.0.0.1");

            // enable known dependency tracking, note that in future versions, we will extend this list. 
            // please check default settings in https://github.com/microsoft/ApplicationInsights-dotnet-server/blob/develop/WEB/Src/DependencyCollector/DependencyCollector/ApplicationInsights.config.install.xdt

            module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
            module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

            // initialize the module
            module.Initialize(configuration);

            return module;
        }
    }
}
