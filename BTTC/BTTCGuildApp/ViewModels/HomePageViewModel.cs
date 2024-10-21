using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Avalonia.Collections;
using BTTCGuildApp.Helpers;
using BTTCGuildApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTTCGuildApp.ViewModels
{
    public partial class HomePageViewModel : ViewModelBase
    {
        public string[] ArchetypeList { get; } = Enum.GetValues<Archetype>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isArchetypeListOpen = false;
        public AvaloniaList<string> ArchetypeSelectedList { get; } = [];
        [ObservableProperty] private string _archetypeSelectedListTooltip = "None";
        [RelayCommand] private void ToggleArchetypeList() => IsArchetypeListOpen ^= true;
        [RelayCommand] private void ClearArchetypeSelectedList() => RaceSelectedList.Clear();

        public string[] ArtisanList { get; } = Enum.GetValues<Artisan>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isArtisanListOpen = false;
        public AvaloniaList<string> ArtisanSelectedList { get; } = [];
        [ObservableProperty] private string _artisanSelectedListTooltip = "None";
        [RelayCommand] private void ToggleArtisanList() => IsArtisanListOpen ^= true;
        [RelayCommand] private void ClearArtisanSelectedList() => ArtisanSelectedList.Clear();

        public string[] BranchList { get; } = Enum.GetValues<GuildBranch>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isBranchListOpen = false;
        public AvaloniaList<string> BranchSelectedList { get; } = [];
        [ObservableProperty] private string _branchSelectedListTooltip = "None";
        [RelayCommand] private void ToggleBranchList() => IsBranchListOpen ^= true;
        [RelayCommand] private void ClearBranchSelectedList() => BranchSelectedList.Clear();

        public string[] TimezoneList { get; } = Enum.GetValues<Timezone>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isTimezoneListOpen = false;
        public AvaloniaList<string> TimezoneSelectedList { get; } = [];
        [ObservableProperty] private string _timezoneSelectedListTooltip = "None";
        [RelayCommand] private void ToggleTimezoneList() => IsTimezoneListOpen ^= true;
        [RelayCommand] private void ClearTimezoneSelectedList() => TimezoneSelectedList.Clear();

        public string[] RaceList { get; } = Enum.GetValues<Race>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isRaceListOpen = false;
        public AvaloniaList<string> RaceSelectedList { get; } = [];
        [ObservableProperty] private string _raceSelectedListTooltip = "None";
        [RelayCommand] private void ToggleRaceList() => IsRaceListOpen ^= true;
        [RelayCommand] private void ClearRaceSelectedList() => RaceSelectedList.Clear();

        public string[] RolePlayPriorityList { get; } = Enum.GetValues<RolePlayPriority>().Select(x => x.GetStringValue()).ToArray()!;
        [ObservableProperty] private bool _isRolePlayPriorityListOpen = false;
        public AvaloniaList<string> RolePlayPrioritySelectedList { get; } = [];
        [ObservableProperty] private string _rolePlayPrioritySelectedListTooltip = "None";
        [RelayCommand] private void ToggleRolePlayPriorityList() => IsRolePlayPriorityListOpen ^= true;
        [RelayCommand] private void ClearRolePlayPrioritySelectedList() => RolePlayPrioritySelectedList.Clear();

        public static AvaloniaList<GuildMemberModel> QueriedMemberList { get; } = [];
        [ObservableProperty] private bool _isDataGridVisible = false;
        // [RelayCommand] private async Task StartGuildSheetSearchAsync()
        // {
        //     KeyValuePair<List<string>, List<GuildMemberModel>>? sheetData = await GuildSpreadsheet.GetGenericInfoSheetData();
        //     if (sheetData is not null)
        //     {
        //         AnalyzedList.Clear();
        //         AnalyzedList.AddRange(sheetData.Value.Value);
        //     }
        // }

        public HomePageViewModel()
        {
            ArchetypeSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionTooltipUpdate(ArchetypeSelectedList,"Archetype");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            ArtisanSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionTooltipUpdate(ArtisanSelectedList, "Artisan");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            BranchSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionTooltipUpdate(BranchSelectedList, "Branch");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            TimezoneSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionTooltipUpdate(TimezoneSelectedList, "Timezone");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            RolePlayPrioritySelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionTooltipUpdate(RolePlayPrioritySelectedList, "RolePlayPriority");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private void CollectionTooltipUpdate(AvaloniaList<string> data, string listName)
        {
            // Early exit if data is null or empty
            if (data is null || data.Count == 0)
            {
                SetTooltipForListName(listName, "None");
                return;
            }

            // Sort the data once
            var sortedData = data.OrderBy(x => x).ToList();
            
            // Use StringBuilder to efficiently build the tooltip string
            var newToolTip = new StringBuilder();
            foreach (var item in sortedData)
            {
                newToolTip.AppendLine(item);
            }

            // Trim the tooltip string and set it to the appropriate property
            SetTooltipForListName(listName, newToolTip.ToString().Trim());
            LOGGER.Debug($"Updating \"{listName}\" Selections to \"{newToolTip}\"");
        }

        private void SetTooltipForListName(string listName, string tooltip)
        {
            // Use a dictionary to map listName to the corresponding property setter
            var tooltipDictionary = new Dictionary<string, Action<string>>
            {
                { "Archetype", tooltip => ArchetypeSelectedListTooltip = tooltip },
                { "Artisan", tooltip => ArtisanSelectedListTooltip = tooltip },
                { "Branch", tooltip => BranchSelectedListTooltip = tooltip },
                { "Timezone", tooltip => TimezoneSelectedListTooltip = tooltip },
                { "RolePlayPriority", tooltip => RolePlayPrioritySelectedListTooltip = tooltip }
            };

            // Set the tooltip if the listName exists in the dictionary
            if (tooltipDictionary.ContainsKey(listName))
            {
                tooltipDictionary[listName](tooltip);
            }
        }
    }
}