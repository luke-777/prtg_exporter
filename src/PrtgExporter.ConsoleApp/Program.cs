using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrtgExporter.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("PRTG-Exporter");


            // Create Configuration:
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            var config = configurationBuilder
                .AddJsonFile("prtgexporter.json", true)
                .AddEnvironmentVariables().Build();

            // Get Prtg & Exporter Options from Configuration
            PrtgOptions prtgOptions = config.GetSection(PrtgOptions.Key).Get<PrtgOptions>();
            ExporterOptions exporterOptions = config.GetSection(ExporterOptions.Key).Get<ExporterOptions>();

            if (prtgOptions == null || exporterOptions == null)
            {
                Console.WriteLine("No configuration is given!");
                return;
            }

            Console.WriteLine($"Connecting to {prtgOptions.Server} with username {prtgOptions.Username}...");
            Console.WriteLine($"Metrics: http://localhost:{exporterOptions.Port}/metrics");

            // Create exporter with PRTG- and Exporter options
            PrtgExporter exporter = new PrtgExporter(prtgOptions, exporterOptions);




            while (true)
            {
                Console.WriteLine($"{DateTimeOffset.Now} - Refreshing...");



                await exporter.RefreshSensorValuesAsync();


                // sleep 10 sec.
                Thread.Sleep(TimeSpan.FromSeconds(10));


            }
        }




    }
}
