using Avalonia.Controls;
using Avalonia.Input;
using BTTCGuildApp.ViewModels;

namespace BTTCGuildApp.Views;

public partial class HomePageView : UserControl
{
    public HomePageView()
    {
        InitializeComponent();
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
        };
    }
}