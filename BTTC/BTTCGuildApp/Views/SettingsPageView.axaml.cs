using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BTTCGuildApp.Views;

public partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
        LOGGER.Debug("Settings Page Initialized");
    }
}