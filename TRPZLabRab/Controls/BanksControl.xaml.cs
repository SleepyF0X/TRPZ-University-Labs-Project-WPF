using System.Windows;
using System.Windows.Controls;
using TRPZLabRab.Animations;

namespace TRPZLabRab.Controls
{
    /// <summary>
    ///     Interaction logic for StoresIndexControl.xaml
    /// </summary>
    public partial class BanksControl : UserControl
    {
        public BanksControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.FadeIn();
        }
    }
}