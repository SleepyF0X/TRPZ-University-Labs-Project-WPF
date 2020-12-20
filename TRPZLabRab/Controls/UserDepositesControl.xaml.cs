using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TRPZLabRab.Animations;

namespace TRPZLabRab.Controls
{
    /// <summary>
    /// Interaction logic for UserDepositesControl.xaml
    /// </summary>
    public partial class UserDepositesControl : UserControl
    {
        public UserDepositesControl()
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
