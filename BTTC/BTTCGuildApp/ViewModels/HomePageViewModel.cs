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
        public string[] ArchetypeList { get; } = Enum.GetValues<Archetype>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isArchetypeListOpen = false;
        public AvaloniaList<string> ArchetypeSelectedList { get; } = [];
        [ObservableProperty] private string _archetypeSelectedListTooltip = "None";
        [RelayCommand] private void ToggleArchetypeList() => IsArchetypeListOpen ^= true;
        [RelayCommand] private void ClearArchetypeSelectedList() => ArchetypeSelectedList.Clear();

        public string[] ArtisanList { get; } = Enum.GetValues<Artisan>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isArtisanListOpen = false;
        public AvaloniaList<string> ArtisanSelectedList { get; } = [];
        [ObservableProperty] private string _artisanSelectedListTooltip = "None";
        [RelayCommand] private void ToggleArtisanList() => IsArtisanListOpen ^= true;
        [RelayCommand] private void ClearArtisanSelectedList() => ArtisanSelectedList.Clear();

        public string[] BranchList { get; } = Enum.GetValues<GuildBranch>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isBranchListOpen = false;
        public AvaloniaList<string> BranchSelectedList { get; } = [];
        [ObservableProperty] private string _branchSelectedListTooltip = "None";
        [RelayCommand] private void ToggleBranchList() => IsBranchListOpen ^= true;
        [RelayCommand] private void ClearBranchSelectedList() => BranchSelectedList.Clear();

        public string[] TimezoneList { get; } = Enum.GetValues<Timezone>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isTimezoneListOpen = false;
        public AvaloniaList<string> TimezoneSelectedList { get; } = [];
        [ObservableProperty] private string _timezoneSelectedListTooltip = "None";
        [RelayCommand] private void ToggleTimezoneList() => IsTimezoneListOpen ^= true;
        [RelayCommand] private void ClearTimezoneSelectedList() => TimezoneSelectedList.Clear();

        public string[] RaceList { get; } = Enum.GetValues<Race>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isRaceListOpen = false;
        public AvaloniaList<string> RaceSelectedList { get; } = [];
        [ObservableProperty] private string _raceSelectedListTooltip = "None";
        [RelayCommand] private void ToggleRaceList() => IsRaceListOpen ^= true;
        [RelayCommand] private void ClearRaceSelectedList() => RaceSelectedList.Clear();

        public string[] RolePlayPriorityList { get; } = Enum.GetValues<RolePlayPriority>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isRolePlayPriorityListOpen = false;
        public AvaloniaList<string> RolePlayPrioritySelectedList { get; } = [];
        [ObservableProperty] private string _rolePlayPrioritySelectedListTooltip = "None";
        [RelayCommand] private void ToggleRolePlayPriorityList() => IsRolePlayPriorityListOpen ^= true;
        [RelayCommand] private void ClearRolePlayPrioritySelectedList() => RolePlayPrioritySelectedList.Clear();

        public string[] IdealNodeList { get; } = Enum.GetValues<Node>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isIdealNodeListOpen = false;
        public AvaloniaList<string> IdealNodeSelectedList { get; } = [];
        [ObservableProperty] private string _IdealNodeSelectedListTooltip = "None";
        [RelayCommand] private void ToggleIdealNodeList() => IsIdealNodeListOpen ^= true;
        [RelayCommand] private void ClearIdealNodeSelectedList() => IdealNodeSelectedList.Clear();

        public string[] AccessLevelList { get; } = Enum.GetValues<AccessLevel>().Select(x => x.GetStringValue()).OrderBy(x => x).ToArray()!;
        [ObservableProperty] private bool _isAccessLevelListOpen = false;
        public AvaloniaList<string> AccessLevelSelectedList { get; } = [];
        [ObservableProperty] private string _AccessLevelSelectedListTooltip = "None";
        [RelayCommand] private void ToggleAccessLevelList() => IsAccessLevelListOpen ^= true;
        [RelayCommand] private void ClearAccessLevelSelectedList() => AccessLevelSelectedList.Clear();

        public static AvaloniaList<GuildMemberModel> QueriedMemberList { get; } = [];
        [ObservableProperty] private string _searchQuery = "All";
        [ObservableProperty] private bool _isDataGridVisible = false;
        [ObservableProperty] private bool _isSearchWarningMessageVisible = false;

        public HomePageViewModel()
        {
            ArchetypeSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(ArchetypeSelectedList,"Archetype");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            ArtisanSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(ArtisanSelectedList,"Artisan");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            BranchSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(BranchSelectedList,"Branch");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            TimezoneSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(TimezoneSelectedList,"Timezone");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            RolePlayPrioritySelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(RolePlayPrioritySelectedList,"RolePlayPriority");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            AccessLevelSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(AccessLevelSelectedList,"AccessLevel");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
                }
            };

            IdealNodeSelectedList.CollectionChanged += (sender, e) => {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        CollectionTooltipUpdate(IdealNodeSelectedList,"IdealNode");
                        return;
                    default:
                        LOGGER.Warn($"Unhandled Action: \"{e.Action}\"");
                        return;
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
            LOGGER.Debug($"Updating \"{listName}\" Selections to \"{newToolTip.Replace("\n", ", ").ToString()[..^2]}\"");
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
                { "RolePlayPriority", tooltip => RolePlayPrioritySelectedListTooltip = tooltip },
                { "AccessLevel", tooltip => AccessLevelSelectedListTooltip = tooltip },
                { "IdealNode", tooltip => IdealNodeSelectedListTooltip = tooltip }
            };

            // Set the tooltip if the listName exists in the dictionary
            if (tooltipDictionary.ContainsKey(listName))
            {
                tooltipDictionary[listName](tooltip);
            }
        }
    }
}