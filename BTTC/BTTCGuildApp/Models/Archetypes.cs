using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public enum Archetypes
    {
        [StringValue("Bard")] Bard,
        [StringValue("Cleric")] Cleric,
        [StringValue("Fighter")] Fighter,
        [StringValue("Mage")] Mage,
        [StringValue("Ranger")] Ranger,
        [StringValue("Rogue")] Rogue,
        [StringValue("Summoner")] Summoner,
        [StringValue("Tank")] Tank,
    }
}