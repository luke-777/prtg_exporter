using Prometheus;
using PrtgAPI;
using System;
using System.Threading.Tasks;

namespace PrtgExporter.ConsoleApp
{
    public class PrtgExporter
    {
        private PrtgOptions _prtgOptions;
        private ExporterOptions _exporterOptions;
        private PrtgClient _prtgClient;
        private MetricServer _MetricServer;

        // Gauge for MetricServer
        private static readonly Gauge SensorValueGauge = Metrics
            .CreateGauge("prtg_sensor_lastvalue", "Last Value of the sensor.",
                new GaugeConfiguration
                {
                    LabelNames = new[] { "id" }
                });


        public PrtgExporter(PrtgOptions prtgOptions, ExporterOptions exporterOptions)
        {
            _prtgOptions = prtgOptions;
            _exporterOptions = exporterOptions;

            // create the API Client:
            _prtgClient = CreatePrtgClient(
                _prtgOptions.Server,
                _prtgOptions.Username,
                _prtgOptions.Password);

            // start the MetricServer:
            _MetricServer = new MetricServer("localhost", exporterOptions.Port);
            _MetricServer.Start();
        }

        /// <summary>
        /// Creates and returns the PRTG-API Client
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="authMode"></param>
        /// <returns>Returns the PrtgClient or null if server is down.</returns>
        private PrtgClient CreatePrtgClient(string server, string user, string password, AuthMode authMode = AuthMode.Password)
        {
            try
            {
                return new PrtgClient(server, user, password, authMode);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't connect to PRTG Server {server}");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Refreshes the Gauges-List
        /// </summary>
        /// <returns></returns>
        public async Task RefreshSensorValuesAsync()
        {
            // check if PrtgClient is null, try to connect again
            if (_prtgClient == null)
                _prtgClient = CreatePrtgClient(
                    _prtgOptions.Server,
                    _prtgOptions.Username,
                    _prtgOptions.Password);

            // if PrtgClient is still null ==> no connection ==> skip!
            if (_prtgClient == null)
                return;


            // get all sensors in prtg:
            var sensors = await _prtgClient.GetSensorsAsync();


            // refresh the sensors in PrometheusGauges
            foreach (var sensor in sensors)
            {
                // set Gauge with id = sensor.id to LastValue
                SensorValueGauge.WithLabels(sensor.Id.ToString()).Set(sensor.LastValue ?? 0);
            }
        }
    }
}
