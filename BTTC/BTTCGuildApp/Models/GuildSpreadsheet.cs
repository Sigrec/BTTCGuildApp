using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Logging;
using BTTCGuildApp.Helpers;

namespace BTTCGuildApp.Models
{
    public class GuildSpreadsheet
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public const string URL = "https://docs.google.com/spreadsheets/d/1BPuFezUHKC1mJduXt0SiY3d8f6r98fEVczg9PaCQ14g/gviz/tq?tqx=out:csv&headers=0";
        public const byte START_INDEX = 2;
        public const short END_INDEX = 503;
        public const string GENERIC_SHEET = "Generic Info";
        public const string ARTISAN_SHEET = "Artisans";
        public const string ARCHETYPE_SHEET = "Archetypes";
        public const char GENERIC_SHEET_START_COLUMN = 'A';
        public const char GENERIC_SHEET_END_COLUMN = 'G';
        public const char ARCHETYPE_SHEET_START_COLUMN = 'A';
        public const char ARCHETYPE_SHEET_END_COLUMN = 'E';
        public const char ARTISAN_SHEET_START_COLUMN = 'A';
        public const char ARTISAN_SHEET_END_COLUMN = 'D';

        public static async Task<KeyValuePair<List<string>, List<GuildMemberModel>>?> GetSheetData( 
            AvaloniaList<string> branch,
            AvaloniaList<string> race, 
            AvaloniaList<string> timezone, 
            AvaloniaList<string> rpPriority,
            AvaloniaList<string> artisan,
            AvaloniaList<string> primaryArchetype,
            string memberName = "")
        {
            try
            {
                bool getAll = !string.IsNullOrWhiteSpace(memberName) || (branch.Count == 0 && race.Count == 0 && timezone.Count == 0 && rpPriority.Count == 0 && artisan.Count == 0 && primaryArchetype.Count == 0);
                List<Task<string>> sheetCalls = [];
                if (getAll || branch.Count > 0 || race.Count > 0 || timezone.Count > 0 || rpPriority.Count > 0)
                {
                    string genericSheetQuery = $"&sheet='{GENERIC_SHEET}'&range={GENERIC_SHEET_START_COLUMN}{START_INDEX}:{GENERIC_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD%2CE%2CF%2CG";
                    LOGGER.Info($"Making API Call to Generic Sheet: \"{URL}{genericSheetQuery}\"");
                    sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{genericSheetQuery}"));
                    LOGGER.Debug($"API Call to Generic Sheet Finished");
                }

                if (getAll || artisan.Count > 0)
                {
                    string artisanSheetQuery = $"&sheet={ARTISAN_SHEET}&range={ARTISAN_SHEET_START_COLUMN}{START_INDEX}:{ARTISAN_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD";
                    LOGGER.Info($"Making API Call to Artisan Sheet: \"{URL}{artisanSheetQuery}\"");
                    sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{artisanSheetQuery}"));
                    LOGGER.Debug($"API Call to Artisan Sheet Finished");
                }

                if (getAll || primaryArchetype.Count > 0)
                {   
                    string archetypeSheetQuery = $"&sheet={ARCHETYPE_SHEET}&range={ARCHETYPE_SHEET_START_COLUMN}{START_INDEX}:{ARCHETYPE_SHEET_END_COLUMN}{END_INDEX}&tq=SELECT%20A%2CB%2CC%2CD%2CE";
                    LOGGER.Info($"Making API Call to Archetype Sheet: \"{URL}{archetypeSheetQuery}\"");
                    sheetCalls.Add(_httpClient.GetStringAsync($"{URL}{archetypeSheetQuery}"));
                    LOGGER.Debug($"API Call to Archetype Sheet Finished");
                }
                string[] responses = await Task.WhenAll(sheetCalls);
                var parsedData = ParseCsvData(responses);

                return new KeyValuePair<List<string>, List<GuildMemberModel>>(parsedData.Key, FilterMembers(
                    parsedData.Value,
                    branch,
                    race, 
                    timezone, 
                    rpPriority, 
                    artisan,
                    primaryArchetype,
                    memberName
                ));
            }
            catch (HttpRequestException e)
            {
                LOGGER.Warn($"Spreadsheet Request Error: {e.Message}");
            }
            return null;
        }

        private static string GetQuery(List<string> response, string column)
        {
            if (response == null || response.Count == 0)
            {
                return string.Empty;
            }
            return string.Join("%20OR%20", response.Select(r => $"({column}%20like%20'{r}')"));
        }

        public static List<GuildMemberModel> FilterMembers(
            List<GuildMemberModel> members,
            AvaloniaList<string> branch, 
            AvaloniaList<string> race, 
            AvaloniaList<string> timezone, 
            AvaloniaList<string> rpPriority,
            AvaloniaList<string> artisan,
            AvaloniaList<string> primaryArchetype,
            string memberName = "")
        {
            return members
                .Where(member =>
                    (string.IsNullOrWhiteSpace(memberName) || member.DiscordName.Contains(memberName, StringComparison.OrdinalIgnoreCase) || member.CharacterName.Contains(memberName, StringComparison.OrdinalIgnoreCase)) &&
                    (branch.Count == 0 || branch.Contains(member.Branch)) &&
                    (race.Count == 0 || race.Contains(member.Race)) &&
                    (timezone.Count == 0 || timezone.Contains(member.Timezone)) &&
                    (artisan.Count == 0 || artisan.Contains(member.GrandmasterArtisanOne) || artisan.Contains(member.GrandmasterArtisanTwo)) &&
                    (primaryArchetype.Count == 0 || primaryArchetype.Contains(member.PrimaryArchetype)) &&
                    (rpPriority.Count == 0 || rpPriority.Contains(member.RpPriority))
                ).ToList();
        }

        private static KeyValuePair<List<string>, List<GuildMemberModel>> ParseCsvData(string[] csvDataList)
        {
            IEnumerable<string> headersConcat = [];
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

            List<string> headers = [.. csvData[0]];
            if (headers.Contains("Class (Auto-populated)"))
            {
                headers[headers.IndexOf("Class (Auto-populated)")] = "Class";
            }
            LOGGER.Debug($"Headers: [ {string.Join(',', headers)} ]");
            HashSet<string> headerSet = [.. headers];

            List<GuildMemberModel> result = [];
            foreach (string[] entry in csvData.Skip(1))
            {
                result.Add(new GuildMemberModel(
                    headerSet.Contains("Discord Name") ? entry[0] : string.Empty,
                    headerSet.Contains("Character Name") ? entry[1] : string.Empty,
                    headerSet.Contains("Branch") ? entry[headers.IndexOf("Branch")]: string.Empty,
                    headerSet.Contains("Ideal Node Type") ? entry[headers.IndexOf("Ideal Node Type")] : string.Empty,
                    headerSet.Contains("Race") ? entry[headers.IndexOf("Race")] : string.Empty,
                    headerSet.Contains("Timezone") ? entry[headers.IndexOf("Timezone")] : string.Empty,
                    headerSet.Contains("Grandmaster 1") ? TrimAfterDash(entry[headers.IndexOf("Grandmaster 1")]) : string.Empty,
                    headerSet.Contains("Grandmaster 2") ? TrimAfterDash(entry[headers.IndexOf("Grandmaster 2")]) : string.Empty,
                    headerSet.Contains("Primary Archetype") ? entry[headers.IndexOf("Primary Archetype")] : string.Empty,
                    headerSet.Contains("Secondary Archetype") ? entry[headers.IndexOf("Secondary Archetype")] : string.Empty,
                    headerSet.Contains("Class") ? entry[headers.IndexOf("Class")] : string.Empty,
                    headerSet.Contains("RP Priority") ? entry[headers.IndexOf("RP Priority")] : string.Empty
                ));
            }

            LOGGER.Debug("Members:");
            foreach (GuildMemberModel member in result)
            {
                LOGGER.Debug(member.ToArrayString());
            }

            return new KeyValuePair<List<string>, List<GuildMemberModel>>(headers, result);
        }

        private static List<string[]> CleanCsvData(string csvData)
        {
            StringBuilder csvClean = new StringBuilder(csvData.Length);
            foreach (char c in csvData)
            {
                if (c != '"')
                {
                    csvClean.Append(c);
                }
            }
            csvData = csvClean.ToString();
            string[] splitData = csvData.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
            List<string[]> result = [];
            foreach (string row in splitData)
            {
                result.Add(row.Split(','));
            }
            return result;
        }
        
        public static string TrimAfterDash(string input)
        {
            int index = input.IndexOf(" - ");
            if (index != -1)
            {
                return input[..index];
            }
            return input;
        }
    }

    public class GuildMemberModel
    {
        public string DiscordName { get; set; }
        public string CharacterName { get; set; }
        public string Branch { get; set; }
        public string IdealNodeType { get; set; }
        public string Race { get; set; }
        public string Timezone { get; set; }
        public string GrandmasterArtisanOne { get; set; }
        public string GrandmasterArtisanTwo { get; set; }
        public string PrimaryArchetype { get; set; }
        public string SecondaryArchetype { get; set; }
        public string Class { get; set; }
        public string RpPriority { get; set; }
        
        public GuildMemberModel(
            string discordName,
            string characterName,
            string branch,
            string idealNodeType,
            string race,
            string timezone,
            string grandmasterArtisanOne,
            string grandmasterArtisanTwo,
            string primaryArchetype,
            string secondaryArchetype,
            string classType,
            string rpPriority)
        {
            DiscordName = discordName;
            CharacterName = characterName;
            Branch = branch;
            IdealNodeType = idealNodeType;
            Race = race;
            Timezone = timezone;
            GrandmasterArtisanOne = grandmasterArtisanOne;
            GrandmasterArtisanTwo = grandmasterArtisanTwo;
            PrimaryArchetype = primaryArchetype;
            SecondaryArchetype = secondaryArchetype;
            Class = classType;
            RpPriority = rpPriority;
        }

        public override string ToString()
        {
            return $@"
                DiscordName: {DiscordName}
                CharacterName: {CharacterName}
                Branch: {Branch}
                IdealNodeType: {IdealNodeType}
                Race: {Race}
                Timezone: {Timezone}
                GrandmasterArtisanOne: {GrandmasterArtisanOne}
                GrandmasterArtisanTwo: {GrandmasterArtisanTwo}
                PrimaryArchetype: {PrimaryArchetype}
                SecondaryArchetype: {SecondaryArchetype}
                Class: {Class}
                RpPriority: {RpPriority}
                ";
        }

        public string ToArrayString()
        {
            return $"[{string.Join(", ", [
                DiscordName,
                CharacterName,
                Branch,
                IdealNodeType,
                Race,
                Timezone,
                GrandmasterArtisanOne,
                GrandmasterArtisanTwo,
                PrimaryArchetype,
                SecondaryArchetype,
                Class,
                RpPriority
            ])}]";
        }
    }
}