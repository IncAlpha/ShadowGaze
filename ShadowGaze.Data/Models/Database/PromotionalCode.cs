using System.Diagnostics.CodeAnalysis;

namespace ShadowGaze.Data.Models.Database;

public class PromotionalCode : BaseDatabaseModel
{
    [NotNull] public string Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan Duration { get; set; }
}