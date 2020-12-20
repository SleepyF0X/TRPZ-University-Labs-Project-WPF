using System.Windows;
using System.Windows.Controls;
using TRPZLabRab.Animations;

namespace TRPZLabRab.Controls.Identity
{
    /// <summary>
    ///     Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl
    {
        public RegisterControl()
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