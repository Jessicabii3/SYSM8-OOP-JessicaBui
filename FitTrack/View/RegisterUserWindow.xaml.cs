using FitTrack.Model;
using FitTrack.ViewModel;
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

namespace FitTrack.View
{
    /// <summary>
    /// Interaction logic for RegisterUserWindow.xaml
    /// </summary>
    public partial class RegisterUserWindow : Window
    {
        public RegisterUserWindow(ManageUser userManager)
        {
            InitializeComponent();
            // Sätter datakontexten till ViewModel
            DataContext = new RegisterUserViewModel(CloseWindow, userManager);
        }
        // Metod som stänger fönstret efter registrering
        private void CloseWindow()
        {
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterUserViewModel viewModel)
            {
                // Uppdaterar lösenord i ViewModel när användaren skriver i PasswordBox
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterUserViewModel viewModel)
            {
                // Uppdaterar ConfirmPassword i ViewModel när användaren skriver i ConfirmPasswordBox
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
