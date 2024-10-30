using FitTrack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.Commands;
using FitTrack.Helpers;
using System.Windows;
using FitTrack.View;
namespace FitTrack.ViewModel

{
    public class MainViewModel:BaseViewModel
    {
        // Egenskaper
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoggedIn;
        private readonly ManageUser _userManager;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand LoginCommand => new RelayCommand(execute => SignIn());
        public RelayCommand ResetPasswordCommand => new RelayCommand(execute => OpenResetPasswordWindow());
        public RelayCommand RegisterUserCommand => new RelayCommand(execute => OpenRegisterUserWindow());

        public MainViewModel(ManageUser userManager)
        {
            _username = string.Empty;
            _password = string.Empty;
            _errorMessage = string.Empty;
            _userManager = ManageUser.Instance;
            _isLoggedIn = false;
        }
        // Metod för att hantera inloggning
        public void SignIn()
        {
            // Validera användarnamnet
            if (!ValidationHelper.IsValidUsername(Username))
            {
                ErrorMessage = "Användarnamnet måste vara minst 3 tecken långt.";
                return;
            }

            // Validera lösenordet
            if (!ValidationHelper.IsValidPassword(Password))
            {
                ErrorMessage = "Lösenordet måste vara minst 8 tecken långt och innehålla minst en siffra och ett specialtecken.";
                return;
            }

            // Kontrollera inloggningsuppgifter
            if (_userManager.ValidateCredentials(Username, Password))
            {
                IsLoggedIn = true;
                var loggedInUser = _userManager.LoggedInUser;

                if (loggedInUser != null)
                {
                    OpenWorkoutWindow(loggedInUser.IsAdmin);
                    Application.Current.MainWindow.Close();
                }
            }
            else
            {
                ErrorMessage = "Ogiltigt användarnamn eller lösenord.";
            }
        }
        // Metod för att öppna WorkoutWindow baserat på användartyp
        private void OpenWorkoutWindow(bool isAdmin)
        {
            var workoutViewModel = new WorkoutsViewModel(_userManager);
            var workoutWindow = new WorkoutsWindow(workoutViewModel);

            if (isAdmin)
            {
                // Specifika inställningar eller åtgärder för admin
                workoutWindow.Title = "Admin Workout Window";
            }
            else
            {
                workoutWindow.Title = "User Workout Window";
            }

            workoutWindow.Show();
        }

        // Metod för att öppna ResetPasswordWindow
        public void OpenResetPasswordWindow()
        {
            var resetPasswordWindow = new ResetPasswordWindow(_userManager);
            resetPasswordWindow.Show();
            Application.Current.MainWindow.Close();
        }

        // Metod för att öppna RegisterWindow
        public void OpenRegisterUserWindow()
        {
            var registerWindow = new RegisterUserWindow(_userManager);
            registerWindow.Show();
            Application.Current.MainWindow.Close();
        }
    }
}
