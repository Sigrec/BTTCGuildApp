using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using BTTCGuildApp.Models;
using BTTCGuildApp.ViewModels;

namespace BTTCGuildApp.Views;

public partial class HomePageView : UserControl
{

    private static readonly Dictionary<string, string> columnMappings = new Dictionary<string, string>
    {
        { "Character Name", "CharacterName" },
        { "Branch", "Branch" },
        { "Access Level", "AccessLevel" },
        { "Race", "Race" },
        { "Timezone", "Timezone" },
        { "Grandmaster 1", "GrandmasterArtisanOne" },
        { "Grandmaster 2", "GrandmasterArtisanTwo" },
        { "Primary Archetype", "PrimaryArchetype" },
        { "Secondary Archetype", "SecondaryArchetype" },
        { "Class", "Class" },
        { "Ideal Node", "IdealNode" },
        { "RP Priority", "RpPriority" },
    };

    public HomePageView()
    {
        InitializeComponent();
        LOGGER.Debug("Home Page Initialized");
    }

    private async void StartGuildSheetSearchAsync(object sender, RoutedEventArgs args)
    {
        HomePageViewModel viewModel = (HomePageViewModel)this.DataContext!;
        viewModel.IsSearchWarningMessageVisible = false;
        string memberName = !string.IsNullOrWhiteSpace(MemberNameTextBox.Text) ? MemberNameTextBox.Text.Trim() : string.Empty;

        LOGGER.Info(
            $"\nSearching for Members With" +
            $"\nName: {(!string.IsNullOrWhiteSpace(memberName) ? $"\"{memberName}\"" : "All")}" +
            $"\nBranch: {(viewModel.BranchSelectedList.Count > 0 ? string.Join(", ", viewModel.BranchSelectedList) : "All")}" +
            $"\nAccess Level: {(viewModel.AccessLevelSelectedList.Count > 0 ? string.Join(", ", viewModel.AccessLevelSelectedList) : "All")}" +
            $"\nRace: {(viewModel.RaceSelectedList.Count > 0 ? string.Join(", ", viewModel.RaceSelectedList) : "All")}" + 
            $"\nTimezone: {(viewModel.TimezoneSelectedList.Count > 0 ? string.Join(", ", viewModel.TimezoneSelectedList) : "All")}" + 
            $"\nArtisan: {(viewModel.ArtisanSelectedList.Count > 0 ? string.Join(", ", viewModel.ArtisanSelectedList) : "All")}" + 
            $"\nArchetype: {(viewModel.ArchetypeSelectedList.Count > 0 ? string.Join(", ", viewModel.ArchetypeSelectedList) : "All")}" + 
            $"\nIdeal Node: {(viewModel.IdealNodeSelectedList.Count > 0 ? string.Join(", ", viewModel.IdealNodeSelectedList) : "All")}" + 
            $"\nRole Play Priority: {(viewModel.RolePlayPrioritySelectedList.Count > 0 ? string.Join(", ", viewModel.RolePlayPrioritySelectedList) : "All")}"
        );
        
        IEnumerable<GuildMemberModel>? sheetData = await GuildSpreadsheet.GetSheetData(
            viewModel.BranchSelectedList, 
            viewModel.RaceSelectedList, 
            viewModel.TimezoneSelectedList, 
            viewModel.RolePlayPrioritySelectedList, 
            viewModel.ArtisanSelectedList,
            viewModel.ArchetypeSelectedList,
            viewModel.AccessLevelSelectedList.Count == 1 ? viewModel.AccessLevelSelectedList[0] : null,
            viewModel.IdealNodeSelectedList,
            memberName
        );

        if (sheetData is null || !sheetData.Any())
        {
            viewModel.IsDataGridVisible = false;
            viewModel.IsSearchWarningMessageVisible = true;
            LOGGER.Info("Search Returned no Data");
            return;
        }

        var dataGrid = GuildMemberSearchDataGrid;

        // Clear the existing queried member list and populate it with new data
        HomePageViewModel.QueriedMemberList.Clear();
        HomePageViewModel.QueriedMemberList.AddRange(sheetData);

        // Clear and prepare the DataGrid columns
        dataGrid.Columns.Clear();
        
        // Add default column (Discord Name)
        dataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Discord Name",
            Binding = new Binding("DiscordName"),
            Width = DataGridLength.Auto
        });

        // Add the relevant columns dynamically
        foreach (string header in GuildSpreadsheet.HEADERS.Skip(1))
        {
            if (columnMappings.TryGetValue(header, out string? propertyName))
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = header,
                    Binding = new Binding(propertyName),
                    Width = header == "Ideal Node Type" ? DataGridLength.SizeToHeader : DataGridLength.Auto
                };

                dataGrid.Columns.Add(column);
            }
        }

        // Make sure the DataGrid is visible
        if (!viewModel.IsDataGridVisible)
        {
            viewModel.IsDataGridVisible = true;
        }
    }

    private void UnShowListBoxPopup(object sender, PointerPressedEventArgs args)
    {
        HomePageViewModel viewModel = (HomePageViewModel)DataContext;
        switch (((Panel)sender).Name)
        {
            case "ArchetypePopup":
                viewModel!.IsArchetypeListOpen = false;
                break;
            case "ArtisanPopup":
                viewModel!.IsArtisanListOpen = false;
                break;
            case "BranchPopup":
                viewModel!.IsBranchListOpen = false;
                break;
            case "TimezonePopup":
                viewModel!.IsTimezoneListOpen = false;
                break;
            case "RolePlayPriorityPopup":
                viewModel!.IsRolePlayPriorityListOpen = false;
                break;
            case "RacePopup":
                viewModel!.IsRaceListOpen = false;
                break;
            case "IdealNodePopup":
                viewModel!.IsIdealNodeListOpen = false;
                break;
            case "AccessLevelPopup":
                viewModel!.IsAccessLevelListOpen = false;
                break;
        };
    }
}