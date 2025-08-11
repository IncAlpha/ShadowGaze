using System.Collections.Concurrent;
using BigBroPublicBot.Core.Models;

namespace BigBroPublicBot.Core.Services;

public class SessionContextProvider
{
    private readonly ConcurrentDictionary<long, SessionContext> _userBotContexts = new();

    public SessionContext GetContextForUser(long id)
    {
        return _userBotContexts.GetOrAdd(id, new SessionContext());
    }
}