using System.Diagnostics;

namespace BigBroPublicBot.Core.Services;

public interface IVersionProvider
{
    FileVersionInfo FileVersionInfo { get; }
}