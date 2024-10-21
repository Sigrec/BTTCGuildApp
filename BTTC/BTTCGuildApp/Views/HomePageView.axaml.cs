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
        { "Ideal Node Type", "IdealNodeType" },
        { "Race", "Race" },
        { "Timezone", "Timezone" },
        { "RP Priority", "RpPriority" },
        { "Grandmaster 1", "GrandmasterArtisanOne" },
        { "Grandmaster 2", "GrandmasterArtisanTwo" },
        { "Primary Archetype", "PrimaryArchetype" },
        { "Secondary Archetype", "SecondaryArchetype" },
        { "Class", "Class" },
    };

    public HomePageView()
    {
        InitializeComponent();
        LOGGER.Debug("Home Page Initialized");
    }

    private async void StartGuildSheetSearchAsync(object sender, RoutedEventArgs args)
    {
        HomePageViewModel viewModel = (HomePageViewModel)this.DataContext!;
        KeyValuePair<List<string>, List<GuildMemberModel>>? sheetData = await GuildSpreadsheet.GetSheetData(
            viewModel.BranchSelectedList, 
            viewModel.RaceSelectedList, 
            viewModel.TimezoneSelectedList, 
            viewModel.RolePlayPrioritySelectedList, 
            viewModel.ArtisanSelectedList,
            viewModel.ArchetypeSelectedList,
            !string.IsNullOrWhiteSpace(MemberNameTextBox.Text) ? MemberNameTextBox.Text.Trim() : string.Empty
        );

        if (sheetData is null || sheetData.Value.Value.Count == 0)
        {
            LOGGER.Debug("Spreadsheet Search Returned no Data");
            // TODO - Add some visual when no data is returned
            return;
        }

        var dataGrid = GuildMemberSearchDataGrid;

        // Clear the existing queried member list and populate it with new data
        HomePageViewModel.QueriedMemberList.Clear();
        HomePageViewModel.QueriedMemberList.AddRange(sheetData.Value.Value);

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
        foreach (string header in sheetData.Value.Key.Skip(1))
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
        };
    }
}