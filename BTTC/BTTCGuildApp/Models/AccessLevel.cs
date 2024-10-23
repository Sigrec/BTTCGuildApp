using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public enum AccessLevel
    {
        [StringValue("A1")] A1 = 0,
        [StringValue("A2")] A2 = A1 | 1 << 1,
        [StringValue("A2 Phase 1")] A2Phase1 = A2 | 1 << 2,
        [StringValue("A2 Phase 2")] A2Phase2 = A2Phase1 | 1 << 3,
        [StringValue("A2 Phase 3")] A2Phase3 = A2Phase2 | 1 << 4,
        [StringValue("B1")] B1 = A2Phase3 | 1 << 5,
        [StringValue("B2")] B2 = B1 | 1 << 6,
        [StringValue("Release")] Release = B2 | 1 << 7,
    }
}