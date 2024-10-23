using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Collections;
using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public partial class GuildSpreadsheet
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public const string URL = "https://docs.google.com/spreadsheets/d/1BPuFezUHKC1mJduXt0SiY3d8f6r98fEVczg9PaCQ14g/gviz/tq?tqx=out:csv&headers=0";
        public static readonly HashSet<string> HEADERS = [
            "Discord Name", "Character Name", "Branch", "Timezone", "Access Level", "Grandmaster 1", "Grandmaster 2", "Primary Archetype", "Secondary Archetype", "Class", "Race", "Ideal Node", "RP Priority",
        ];
        public const byte START_INDEX = 2;
        public const short END_INDEX = 503;
        public const string GENERIC_SHEET = "Generic Info";
        public const string ARTISAN_SHEET = "Artisans";
        public const string ARCHETYPE_SHEET = "Archetypes";
        public const char GENERIC_SHEET_START_COLUMN = 'A';
        public const char GENERIC_SHEET_END_COLUMN = 'H';
        public const char ARCHETYPE_SHEET_START_COLUMN = 'A';
        public const char ARCHETYPE_SHEET_END_COLUMN = 'E';
        public const char ARTISAN_SHEET_START_COLUMN = 'A';
        public const char ARTISAN_SHEET_END_COLUMN = 'D';

        [GeneratedRegex(@"""""([^"",]+)""""")] private static partial Regex QuoteRegex();

        public static async Task<IEnumerable<GuildMemberModel>?> GetSheetData( 
            AvaloniaList<string> branch,
            AvaloniaList<string> race, 
            AvaloniaList<string> timezone, 
            AvaloniaList<string> rpPriority,
            AvaloniaList<string> artisan,
            AvaloniaList<string> primaryArchetype,
            string? accessLevel,
            AvaloniaList<string> idealNode,
            string memberName = "")
        {
            try
            {
                List<Task<string>> sheetCalls = [];
                string genericSheetQuery = $"&sheet='{GENERIC_SHEET}'&range={GENERIC_SHEET_START_COLUMN}{START_INDEX}:{GENERIC_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD%2CE%2CF%2CG%2CH";
                LOGGER.Debug($"Making API Call to Generic Sheet: \"{URL}{genericSheetQuery}\"");
                sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{genericSheetQuery}"));

                string artisanSheetQuery = $"&sheet={ARTISAN_SHEET}&range={ARTISAN_SHEET_START_COLUMN}{START_INDEX}:{ARTISAN_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD";
                LOGGER.Debug($"Making API Call to Artisan Sheet: \"{URL}{artisanSheetQuery}\"");
                sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{artisanSheetQuery}"));

                string archetypeSheetQuery = $"&sheet={ARCHETYPE_SHEET}&range={ARCHETYPE_SHEET_START_COLUMN}{START_INDEX}:{ARCHETYPE_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD%2CE";
                LOGGER.Debug($"Making API Call to Archetype Sheet: \"{URL}{archetypeSheetQuery}\"");
                sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{archetypeSheetQuery}"));

                string[] responses = await Task.WhenAll(sheetCalls);
                LOGGER.Debug($"API Calls to Sheet Finished");

                return FilterMembers(
                    ParseCsvData(responses),
                    branch,
                    race, 
                    timezone, 
                    rpPriority, 
                    artisan,
                    primaryArchetype,
                    accessLevel != null ? EnumHelper.GetEnumFromStringValue<AccessLevel>(accessLevel) : null,
                    idealNode,
                    memberName
                );
            }
            catch (HttpRequestException e)
            {
                LOGGER.Warn($"Spreadsheet Request Error: {e.Message}");
            }
            return null;
        }

        public static IEnumerable<GuildMemberModel> FilterMembers(
            List<GuildMemberModel> members,
            AvaloniaList<string> branch, 
            AvaloniaList<string> race, 
            AvaloniaList<string> timezone, 
            AvaloniaList<string> rpPriority,
            AvaloniaList<string> artisan,
            AvaloniaList<string> primaryArchetype,
            AccessLevel? accessLevel,
            AvaloniaList<string> idealNode,
            string memberName = "")
        {
            return members
                .Where(member =>
                    (string.IsNullOrWhiteSpace(memberName) || member.DiscordName.Contains(memberName, StringComparison.OrdinalIgnoreCase) || member.CharacterName.Contains(memberName, StringComparison.OrdinalIgnoreCase)) &&
                    (branch.Count == 0 || branch.Contains(member.Branch)) &&
                    (race.Count == 0 || race.Contains(member.Race)) &&
                    (timezone.Count == 0 || timezone.Contains(member.Timezone)) &&
                    (accessLevel is null || ((accessLevel.Value & EnumHelper.GetEnumFromStringValue<AccessLevel>(member.AccessLevel)) == EnumHelper.GetEnumFromStringValue<AccessLevel>(member.AccessLevel))) &&
                    (artisan.Count == 0 || artisan.Contains(member.GrandmasterArtisanOne) || artisan.Contains(member.GrandmasterArtisanTwo)) &&
                    (primaryArchetype.Count == 0 || primaryArchetype.Contains(member.PrimaryArchetype)) &&
                    (idealNode.Count == 0 || idealNode.Contains(member.IdealNode)) &&
                    (rpPriority.Count == 0 || rpPriority.Contains(member.RpPriority))
                );
        }

        private static List<GuildMemberModel> ParseCsvData(string[] csvDataList)
        {
            List<string[]> csvData = [];
            foreach(string curCsvData in csvDataList)
            {
                List<string[]> cleanedCsvData = CleanCsvData(curCsvData);
                if (csvData.Count == 0)
                {
                    cleanedCsvData[0][0] = "Discord Name";
                    cleanedCsvData[0][1] = "Character Name";
                    csvData.AddRange(cleanedCsvData);
                }
                else
                {
                    csvData = csvData.Zip(cleanedCsvData, (arr1, arr2) => arr1.Concat(arr2.Skip(2)).ToArray()).ToList();
                }
            }

            List<GuildMemberModel> result = [];
            LOGGER.Debug("Members:");
            LOGGER.Debug(string.Join(",", csvData[1]));
            foreach (string[] entry in csvData.Skip(1))
            {
                GuildMemberModel newMember = new GuildMemberModel()
                    .SetDiscordName(entry[0])
                    .SetCharacterName(entry[1])
                    .SetBranch(entry[2])
                    .SetIdealNode(entry[3])
                    .SetRace(entry[4])
                    .SetTimezone(entry[5])
                    .SetRpPriority(entry[6])
                    .SetAccessLevel(entry[7])
                    .SetGrandmasterArtisanOne(entry[8])
                    .SetGrandmasterArtisanTwo(entry[9])
                    .SetPrimaryArchetype(entry[10])
                    .SetSecondaryArchetype(entry[11])
                    .SetClass(entry[12]);
                LOGGER.Debug(newMember.ToArrayString());
                result.Add(newMember);
            }
            return result;
        }

        private static List<string[]> CleanCsvData(string csvData)
        {
            StringBuilder csvClean = new StringBuilder(csvData.Length);
            foreach (char c in QuoteRegex().Replace(csvData, "&quot;$1&quot;"))
            {
                if (c != '"')
                {
                    csvClean.Append(c);
                }
            }
            csvData = HttpUtility.HtmlDecode(csvClean.ToString());
            string[] splitData = csvData.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
            List<string[]> result = new(splitData.Length);
            foreach (string row in splitData)
            {
                result.Add(row.Split(','));
            }
            return result;
        }
    }

    public class GuildMemberModel
    {
        public string DiscordName { get; private set; }
        public string CharacterName { get; private set; }
        public string Branch { get; private set; }
        public string AccessLevel { get; private set; }
        public string Race { get; private set; }
        public string Timezone { get; private set; }
        public string GrandmasterArtisanOne { get; private set; }
        public string GrandmasterArtisanTwo { get; private set; }
        public string PrimaryArchetype { get; private set; }
        public string SecondaryArchetype { get; private set; }
        public string Class { get; private set; }
        public string IdealNode { get; private set; }
        public string RpPriority { get; private set; }

        public GuildMemberModel()
        {
        }

        public static string TrimAfterDash(string input)
        {
            int index = input.IndexOf(" - ");
            if (index != -1)
            {
                return input[..index];
            }
            return input.Trim();
        }

        public GuildMemberModel SetDiscordName(string discordName)
        {
            this.DiscordName = discordName.Trim();
            return this;
        }

        public GuildMemberModel SetCharacterName(string characterName)
        {
            this.CharacterName = characterName.Trim();
            return this;
        }

        public GuildMemberModel SetBranch(string branch)
        {
            this.Branch = branch;
            return this;
        }

        public GuildMemberModel SetAccessLevel(string accessLevel)
        {
            this.AccessLevel = accessLevel;
            return this;
        }

        public GuildMemberModel SetRace(string race)
        {
            this.Race = race;
            return this;
        }

        public GuildMemberModel SetTimezone(string timezone)
        {
            this.Timezone = timezone;
            return this;
        }

        public GuildMemberModel SetGrandmasterArtisanOne(string grandmasterArtisanOne)
        {
            this.GrandmasterArtisanOne = TrimAfterDash(grandmasterArtisanOne);
            return this;
        }

        public GuildMemberModel SetGrandmasterArtisanTwo(string grandmasterArtisanTwo)
        {
            this.GrandmasterArtisanTwo = TrimAfterDash(grandmasterArtisanTwo);
            return this;
        }

        public GuildMemberModel SetPrimaryArchetype(string primaryArchetype)
        {
            this.PrimaryArchetype = primaryArchetype;
            return this;
        }

        public GuildMemberModel SetSecondaryArchetype(string secondaryArchetype)
        {
            this.SecondaryArchetype = secondaryArchetype;
            return this;
        }

        public GuildMemberModel SetClass(string classType)
        {
            this.Class = classType;
            return this;
        }

        public GuildMemberModel SetIdealNode(string idealNode)
        {
            this.IdealNode = idealNode;
            return this;
        }

        public GuildMemberModel SetRpPriority(string rpPriority)
        {
            this.RpPriority = rpPriority;
            return this;
        }

        public override string ToString()
        {
            return $@"
                DiscordName: {DiscordName}
                CharacterName: {CharacterName}
                Branch: {Branch}
                Timezone: {Timezone}
                AccessLevel: {AccessLevel}
                GrandmasterArtisanOne: {GrandmasterArtisanOne}
                GrandmasterArtisanTwo: {GrandmasterArtisanTwo}
                PrimaryArchetype: {PrimaryArchetype}
                SecondaryArchetype: {SecondaryArchetype}
                Class: {Class}
                Race: {Race}
                IdealNodeType: {IdealNode}
                RpPriority: {RpPriority}
                ";
        }

        public string ToArrayString()
        {
            return $"[{string.Join(", ", [
                DiscordName,
                CharacterName,
                Branch,
                Timezone,
                AccessLevel,
                GrandmasterArtisanOne,
                GrandmasterArtisanTwo,
                PrimaryArchetype,
                SecondaryArchetype,
                Race,
                Class,
                IdealNode,
                RpPriority
            ])}]";
        }
    }
}