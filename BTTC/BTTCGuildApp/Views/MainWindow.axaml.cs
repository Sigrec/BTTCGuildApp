using Avalonia.Controls;

namespace BTTCGuildApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                LOGGER.Info("Closing BTTC Guild App");
            };
        }
    }
}