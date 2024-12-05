using Microsoft.Extensions.Configuration;
using PrtgExporter.ConsoleApp.Options;
using System;
using System.Threading.Tasks;

namespace PrtgExporter.ConsoleApp;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("PRTG-Exporter");

        var configurationBuilder = new ConfigurationBuilder();
        var config = configurationBuilder
            .AddJsonFile("prtgexporter.json", true)
            .AddEnvironmentVariables()
            .Build();

        // Get Prtg & Exporter Options from Configuration
        var prtgOptions = config.GetSection(PrtgOptions.Key).Get<PrtgOptions>();
        var exporterOptions = config.GetSection(ExporterOptions.Key).Get<ExporterOptions>();

        if (prtgOptions == null || exporterOptions == null || exporterOptions.RefreshInterval <= 0)
        {
            Console.WriteLine("No configuration is given!");
            return;
        }

        Console.WriteLine($"Connecting to {prtgOptions.Server} with username {prtgOptions.Username}...");
        Console.WriteLine($"Metrics: http://localhost:{exporterOptions.Port}/metrics");

        // Create exporter with PRTG- and Exporter options
        var exporter = new PrtgExporter(prtgOptions, exporterOptions);

        while (true)
        {
            Console.WriteLine($"{DateTimeOffset.Now} - Refreshing...");

            await exporter.RefreshSensorValuesAsync();
            await Task.Delay(TimeSpan.FromSeconds(exporterOptions.RefreshInterval));
        }
    }
}
