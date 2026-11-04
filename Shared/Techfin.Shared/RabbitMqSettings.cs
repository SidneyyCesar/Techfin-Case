namespace Techfin.Shared;

public sealed class RabbitMqSettings
{
    public string HostName { get; set; } = "";

    public string Exchange { get; set; } = "";

    public string Queue { get; set; } = "";
}
