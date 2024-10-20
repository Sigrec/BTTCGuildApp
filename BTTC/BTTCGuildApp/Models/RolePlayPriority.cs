using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public enum RolePlayPriority
    {
        [StringValue("None")] None,
        [StringValue("Low")] Low,
        [StringValue("Medium")] Medium,
        [StringValue("High")] High,
    }
}