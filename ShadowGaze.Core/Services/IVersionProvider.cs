using System.Diagnostics;

namespace ShadowGaze.Core.Services;

public interface IVersionProvider
{
    FileVersionInfo FileVersionInfo { get; }
}