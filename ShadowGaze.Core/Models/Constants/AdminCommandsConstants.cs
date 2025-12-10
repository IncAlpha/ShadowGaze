namespace ShadowGaze.Core.Models.Constants;

public static class AdminCommandsConstants
{
    public const string AddInstruction = "/addinstruction";
    public const string File = "/file";
    public const string Xray = "/xray";
    public const string SendMessage = "/sendmessage";
    
    public static string[] AdminCommands => [AddInstruction, File, Xray, SendMessage];
}