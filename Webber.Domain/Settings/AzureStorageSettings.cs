namespace Webber.Domain.Settings;

/// <summary>
/// Azure Storage settings.
/// </summary>

public class AzureStorageSettings
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}