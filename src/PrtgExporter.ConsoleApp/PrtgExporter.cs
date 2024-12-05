using Prometheus;
using PrtgAPI;
using PrtgExporter.ConsoleApp.Options;
using System;
using System.Threading.Tasks;

namespace PrtgExporter.ConsoleApp;

internal class PrtgExporter
{
    private readonly PrtgOptions _prtgOptions;
    private readonly ExporterOptions _exporterOptions;
    private readonly MetricServer _metricServer;
    private PrtgClient? _prtgClient;

    private static readonly Gauge SensorValueGauge = Metrics
        .CreateGauge("prtg_sensor_lastvalue", "Last Value of the sensor.",
            new GaugeConfiguration
            {
                LabelNames = ["id"]
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
        _metricServer = new MetricServer("localhost", _exporterOptions.Port);
        _metricServer.Start();
    }

    /// <summary>
    /// Creates and returns the PRTG-API Client
    /// </summary>
    /// <returns>Returns the PrtgClient or null if server is down.</returns>
    private static PrtgClient? CreatePrtgClient(string server, string user, string password, AuthMode authMode = AuthMode.Password)
    {
        try
        {
            return new PrtgClient(server, user, password, authMode);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Can't connect to PRTG Server {server}: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Refreshes the Gauges-List
    /// </summary>
    public async Task RefreshSensorValuesAsync()
    {
        // check if PrtgClient is null, try to connect again
        _prtgClient ??= CreatePrtgClient(
                _prtgOptions.Server,
                _prtgOptions.Username,
                _prtgOptions.Password);

        // if PrtgClient is still null ==> no connection ==> skip!
        if (_prtgClient == null)
            return;

        // refresh the sensors in PrometheusGauges
        foreach (var sensor in await _prtgClient.GetSensorsAsync())
        {
            // set Gauge with id = sensor.id to LastValue
            SensorValueGauge.WithLabels(sensor.Id.ToString()).Set(sensor.LastValue ?? 0);
        }
    }
}
