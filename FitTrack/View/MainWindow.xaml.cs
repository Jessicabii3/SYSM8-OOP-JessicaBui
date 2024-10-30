using FitTrack.Model;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ManageUser _userManager;
        public MainWindow(ManageUser userManager)
        {
            
            InitializeComponent();
            _userManager = userManager;
            DataContext=new MainViewModel(_userManager);
        }
        // Standardkonstruktor som använder Singleton-instansen av ManageUser
        public MainWindow() : this(ManageUser.Instance)
        {
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Sätt lösenordet i ViewModel när det ändras i PasswordBox
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
