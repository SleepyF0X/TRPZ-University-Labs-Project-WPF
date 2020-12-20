using System.Windows;
using System.Windows.Controls;
using TRPZLabRab.Animations;

namespace TRPZLabRab.Controls.Identity
{
    /// <summary>
    ///     Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
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