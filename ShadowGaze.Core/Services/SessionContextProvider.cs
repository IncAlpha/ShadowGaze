using System.Collections.Concurrent;
using ShadowGaze.Core.Models.SessionContexts;

namespace ShadowGaze.Core.Services;

public class SessionContextProvider
{
    private readonly ConcurrentDictionary<long, SessionContext> _userBotContexts = new();

    public SessionContext GetContextForUser(long id)
    {
        return _userBotContexts.GetOrAdd(id, new SessionContext());
    }
}