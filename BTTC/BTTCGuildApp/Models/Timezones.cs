using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public enum Timezones
    {
        [StringValue("Atlantic (AST)")] AST,
        [StringValue("Australia (AUS)")] AUS,
        [StringValue("Central (CST)")] CST,
        [StringValue("Eastern (EST)")] EST,
        [StringValue("European (Euro)")] Euro,
        [StringValue("Mountain (MST)")] MST,
        [StringValue("New Zealand (NZ)")] NZ,
        [StringValue("Pacific (PST)")] PST,
    }
}