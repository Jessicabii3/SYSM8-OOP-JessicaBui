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
using System.Windows.Shapes;
using FitTrack.ViewModel;

namespace FitTrack.View
{
    /// <summary>
    /// Interaction logic for UserDetailsWindow.xaml
    /// </summary>
    public partial class UserDetailsWindow : Window
    {
        public UserDetailsWindow(UserDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDetailsViewModel viewModel)
            {
                var passwordBox = sender as PasswordBox;
                if (passwordBox != null)
                {
                    viewModel.NewPassword = passwordBox.Password;
                }
            }
        }

        private void ConfirmpasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDetailsViewModel viewModel)
            {
                var confirmPasswordBox = sender as PasswordBox;
                if (confirmPasswordBox != null)
                {
                    viewModel.ConfirmPassword = confirmPasswordBox.Password;
                }
            }
        }
    }
}
