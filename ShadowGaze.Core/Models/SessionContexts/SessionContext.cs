namespace ShadowGaze.Core.Models.SessionContexts;

public class SessionContext
{
    public IContextState State { get; private set; }
    public int? LastSentMessage { get; set; }

    public void UpdateState(IContextState state)
    {
        State = state;
        LastSentMessage = null;
    }
}