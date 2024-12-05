namespace PrtgExporter.ConsoleApp.Options;

internal class PrtgOptions
{
    public const string Key = "PRTG";
    public string Server { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
