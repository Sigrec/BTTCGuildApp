using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BTTCGuildApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTTCGuildApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private static HomePageView HomePage = new HomePageView() { DataContext = new HomePageViewModel() };
        private static AboutPageView AboutPage = new AboutPageView() { DataContext = new AboutPageViewModel() };
        private static SettingsPageView SettingsPage = new SettingsPageView() { DataContext = new SettingsPageViewModel() };
        [ObservableProperty] private bool _isMenuOpen = false;
        [ObservableProperty] private UserControl? _currentPage = HomePage;
        private static string currentPageLabel = "Home";
        [RelayCommand] private void OpenMenu() => IsMenuOpen ^= true;
        public ObservableCollection<MenuItem> MenuItems { get; } =
        [
            new MenuItem(typeof(HomePageView), "fa-solid fa-house"),
            // new MenuItem(typeof(SettingsPageView), "fa-solid fa-gear"),
            new MenuItem(typeof(AboutPageView), "fa-solid fa-circle-info"),
        ];
        [ObservableProperty] private MenuItem? _selectedMenuItem;

        partial void OnSelectedMenuItemChanged(MenuItem? value)
        {
            if (value is not null && !value.Label.Equals(currentPageLabel))
            {
                LOGGER.Debug($"Menu Changed to \"{value.Label} Page\"");
                currentPageLabel = value.Label;
                CurrentPage = value.Label switch
                {
                    "Home" => HomePage,
                    "About" => AboutPage,
                    "Settings" => SettingsPage,
                    _ => throw new ArgumentNullException()
                };
            }
        }
    }

    public class MenuItem
    {
        public string Label { get; }
        public Type Model { get; }
        public string Icon { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The view model this menu selection goes to</param>
        /// <param name="icon">FontAwesome 6 Icon String</param>
        /// <param name="label">Specific name of the page</param>
        public MenuItem(Type model, string icon, string label = "")
        {
            this.Model = model;
            this.Label = string.IsNullOrWhiteSpace(label) ? model.Name.Replace("PageView", string.Empty) : label;
            this.Icon = icon;
    }
    }
}
