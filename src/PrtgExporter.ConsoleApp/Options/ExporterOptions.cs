namespace PrtgExporter.ConsoleApp.Options;

internal class ExporterOptions
{
    public const string Key = "Exporter";
    public int Port { get; set; }
    public int RefreshInterval { get; set; }
}
