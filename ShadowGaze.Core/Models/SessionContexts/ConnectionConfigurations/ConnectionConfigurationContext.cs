namespace ShadowGaze.Core.Models.SessionContexts.ConnectionConfigurations;

public class ConnectionConfigurationContext : IContextState
{
    public int? SelectedConfigurationId { get; set; }
    public bool WaitingForSni { get; set; }
}
