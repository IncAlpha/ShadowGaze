using ShadowGaze.Data.Models.Database.Instructions;

namespace ShadowGaze.Core.Models.SessionContexts.Instructions;

public class PlatformInstructionContext : IContextState
{
    public PlatformInstruction PlatformInstruction { get; set; }
}