using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    internal class LogDemo : IDisposable
    {
        public readonly ILoggerFactory loggerFactory;

        public LogDemo(string connectionString, TokenCredential credential = null)
        {
            var resourceAttributes = new Dictionary<string, object>
            {
                { "service.name", "my-service" },
                { "service.namespace", "my-namespace" },
                { "service.instance.id", "my-instance" },
                { "service.version", "1.0.0-demo" },
            };

            var resourceBuilder = ResourceBuilder.CreateDefault().AddAttributes(resourceAttributes);
            
            this.loggerFactory = LoggerFactory.Create(builder =>
            { 
                builder.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resourceBuilder);
                    options.AddAzureMonitorLogExporter(o => o.ConnectionString = connectionString, credential);
                }).SetMinimumLevel(LogLevel.Trace);
            });
        }

        /// <remarks>
        /// Logs will be ingested as an Application Insights trace.
        /// These can be differentiated by their severityLevel.
        /// </remarks>
        public void GenerateLogs()
        {
            var logger = this.loggerFactory.CreateLogger<LogDemo>();
            logger.LogInformation("Hello from {name} {price}.", "tomato", 2.99);
            logger.LogError("An error occurred.");
        }

        public void Dispose()
        {
            this.loggerFactory.Dispose();
        }
    }
}
