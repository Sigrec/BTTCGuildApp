using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BTTCGuildApp.Models
{
    public class GuildSpreadsheet
    {
        public const string URL = "https://docs.google.com/spreadsheets/d/1BPuFezUHKC1mJduXt0SiY3d8f6r98fEVczg9PaCQ14g/gviz/tq?tqx=out:csv&headers=0";
        public const byte START_INDEX = 2;
        public const short END_INDEX = 503;
        public const string GENERIC_SHEET = "Generic Info";
        public const string ARTISAN_SHEET = "Artisans";
        public const string ARCHETYPE_SHEET = "Archetypes";
    }

    public class GuildMemberModel
    {
        [JsonPropertyName("Discord Name")]
        public string DiscordName { get; set; }

        [JsonPropertyName("Character Name")]
        public string CharacterName { get; set; }

        [JsonPropertyName("Branch")]
        public string Branch { get; set; }

        [JsonPropertyName("Timezone")]
        public string Timezone { get; set; }
    }
}