using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BTTCGuildApp.Views;

public partial class AboutPageView : UserControl
{
    public AboutPageView()
    {
        InitializeComponent();
        LOGGER.Debug("About Page Initialized");
    }
}