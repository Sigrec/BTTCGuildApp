using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Avalonia.Collections;
using Avalonia.Controls;
using BTTCGuildApp.Helpers;
using BTTCGuildApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTTCGuildApp.ViewModels
{
    public partial class HomePageViewModel : ViewModelBase
    {
        public string[] ArchetypeList { get; } = Enum.GetValues<Archetypes>().Select(x => x.GetStringValue()).ToArray();
        [ObservableProperty] private bool _isArchetypeListOpen = false;
        public AvaloniaList<string> ArchetypeSelectedList { get; }  = [];
        [ObservableProperty] private string _archetypeSelectedListTooltip = string.Empty;
        [RelayCommand] private void ToggleArchetypeList() => IsArchetypeListOpen ^= true;

        public string[] ArtisanList { get; } = Enum.GetValues<Artisans>().Select(x => x.GetStringValue()).ToArray();
        [ObservableProperty] private bool _isArtisanListOpen = false;
        public AvaloniaList<string> ArtisanSelectedList { get; } = [];
        [ObservableProperty] private string _artisanSelectedListTooltip = string.Empty;
        [RelayCommand] private void ToggleArtisanList() => IsArtisanListOpen ^= true;

        public string[] BranchList { get; } = Enum.GetValues<Branches>().Select(x => x.GetStringValue()).ToArray();
        [ObservableProperty] private bool _isBranchListOpen = false;
        public AvaloniaList<string> BranchSelectedList { get; } = [];
        [ObservableProperty] private string _branchSelectedListTooltip = string.Empty;
        [RelayCommand] private void ToggleBranchList() => IsBranchListOpen ^= true;

        public string[] TimezoneList { get; } = Enum.GetValues<Timezones>().Select(x => x.GetStringValue()).ToArray();
        [ObservableProperty] private bool _isTimezoneListOpen = false;
        public AvaloniaList<string> TimezoneSelectedList { get; } = [];
        [ObservableProperty] private string _timezoneSelectedListTooltip = string.Empty;
        [RelayCommand] private void ToggleTimezoneList() => IsTimezoneListOpen ^= true;

        public string[] RolePlayPriorityList { get; } = Enum.GetValues<RolePlayPriority>().Select(x => x.GetStringValue()).ToArray();
        [ObservableProperty] private bool _isRolePlayPriorityListOpen = false;
        public AvaloniaList<string> RolePlayPrioritySelectedList { get; } = [];
        [ObservableProperty] private string _rolePlayPrioritySelectedListTooltip = string.Empty;
        [RelayCommand] private void ToggleRolePlayPriorityList() => IsRolePlayPriorityListOpen ^= true;

        public AvaloniaList<GuildMemberModel> AnalyzedList { get; set; } = new AvaloniaList<GuildMemberModel>();

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
            StringBuilder newToolTip = new StringBuilder();
            if (data != null && data.Any())
            {
                foreach (string y in data.OrderBy(x => x))
                {
                    newToolTip.AppendLine(y);
                }

                switch (listName)
                {
                    case "Archetype":
                        ArchetypeSelectedListTooltip = newToolTip.ToString().Trim();
                        break;
                    case "Artisan":
                        ArtisanSelectedListTooltip = newToolTip.ToString().Trim();
                        break;
                    case "Branch":
                        BranchSelectedListTooltip = newToolTip.ToString().Trim();
                        break;
                    case "Timezone":
                        TimezoneSelectedListTooltip = newToolTip.ToString().Trim();
                        break;
                    case "RolePlayPriority":
                        RolePlayPrioritySelectedListTooltip = newToolTip.ToString().Trim();
                        break;
                }
            }
            else
            {
                switch (listName)
                {
                    case "Archetype":
                        ArchetypeSelectedListTooltip = string.Empty;
                        break;
                    case "Artisan":
                        ArchetypeSelectedListTooltip = string.Empty;
                        break;
                    case "Branch":
                        ArchetypeSelectedListTooltip = string.Empty;
                        break;
                    case "Timezone":
                        ArchetypeSelectedListTooltip = string.Empty;
                        break;
                    case "RolePlayPriority":
                        ArchetypeSelectedListTooltip = string.Empty;
                        break;
                }
            }
            LOGGER.Debug($"Updateing {listName} Selections to \n{newToolTip}");
            newToolTip.Clear();
        }
    }
}