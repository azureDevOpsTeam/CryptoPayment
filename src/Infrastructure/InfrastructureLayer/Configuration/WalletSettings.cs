namespace Infrastructure.InfrastructureLayer.Configuration;

public class WalletSettings
{
    public const string SectionName = "WalletSettings";
    
    public string MasterSeed { get; set; } = string.Empty;
}