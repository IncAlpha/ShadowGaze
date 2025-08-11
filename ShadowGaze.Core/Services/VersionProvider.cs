using System.Diagnostics;
using System.Reflection;

namespace ShadowGaze.Core.Services;

public class VersionProvider : IVersionProvider
{
    public FileVersionInfo FileVersionInfo { get; }

    public VersionProvider()
    {
        var assembly = Assembly.GetEntryAssembly();
        FileVersionInfo = FileVersionInfo.GetVersionInfo(assembly!.Location);
    }
}