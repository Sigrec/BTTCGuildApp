using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTTCGuildApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private bool _isMenuOpen = false;
        [ObservableProperty] private ViewModelBase _currentPage = new HomePageViewModel();
        [RelayCommand] private void OpenMenu() => IsMenuOpen ^= true;
        public ObservableCollection<MenuItem> MenuItems { get; } =
        [
            new MenuItem(typeof(HomePageViewModel), "fa-solid fa-house"),
            new MenuItem(typeof(SettingsPageViewModel), "fa-solid fa-gear"),
            new MenuItem(typeof(AboutPageViewModel), "fa-solid fa-circle-info"),
        ];
        [ObservableProperty] private MenuItem? _selectedMenuItem;

        partial void OnSelectedMenuItemChanged(MenuItem? value)
        {
            if (value is not null)
            {
                var instance = Activator.CreateInstance(value.Model);
                if (instance is not null)
                {
                    CurrentPage = (ViewModelBase)instance;
                }
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
            this.Label = string.IsNullOrWhiteSpace(label) ? model.Name.Replace("PageViewModel", string.Empty) : label;
            this.Icon = icon;
    }
    }
}
