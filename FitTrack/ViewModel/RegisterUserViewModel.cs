using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.View;
using FitTrack.Commands;
using FitTrack.Helpers;
using FitTrack.Model;
using System.Collections.ObjectModel;

namespace FitTrack.ViewModel
{
    public class RegisterUserViewModel : BaseViewModel
    {
        private string? _username;
        private string? _password;
        private string? _confirmPassword;
        private string? _selectedCountry;
        private string? _securityQuestion;
        private string? _securityAnswer;
        private string? _errorMessage;
        private ManageUser _userManager;
        private Action _closeAction;

        // Egenskap för användarnamn
        public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        // Egenskap för lösenord
        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        // Egenskap för bekräftelselösen
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        // Egenskap för valt land
        public string? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged();
            }
        }
        // Egenskap för säkerhetsfråga
        public string? SecurityQuestion
        {
            get => _securityQuestion;
            set
            {
                _securityQuestion = value;
                OnPropertyChanged();
            }
        }
        // Egenskap för säkerhetssvar
        public string? SecurityAnswer
        {
            get => _securityAnswer;
            set
            {
                _securityAnswer = value;
                OnPropertyChanged();
            }
        }
        // Lista över tillgängliga länder för att fylla ComboBox i UI
        public ObservableCollection<string> SelectedCountries { get; set; }
        // Egenskap för att visa felmeddelanden i UI
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
        // Kommando för att registrera användare
        public RelayCommand RegisterUserCommand => new RelayCommand(execute => RegisterUser());

        public RegisterUserViewModel(Action closeAction, ManageUser manager)
        {
            _userManager = manager;
            _closeAction = closeAction;
            SelectedCountries = CountryManager.GetCountries();

            
            if (SelectedCountries.Count == 0)
            {
                ErrorMessage = "Inga länder tillgängliga.";
            }
        }
        // Metod för att hantera registreringen av en ny användare
        private void RegisterUser()
        {
            // Om någon av valideringarna misslyckas, avbryt registreringen
            if (!ValidateUsername() || !ValidatePassword() || !ValidateConfirmPassword() || !ValidateCountry() || !ValidateSecurityQuestionAndAnswer())
            {
                return; 
            }

            // Skapa en ny användare och lägger till den i systemet
            var newUser = new User(
                username: Username,
                password: Password,
                country: SelectedCountry,
                securityQuestion: SecurityQuestion,
                securityAnswer: SecurityAnswer
            );

            _userManager.AddUser(newUser);
            // Öppna MainWindow för inloggning och stäng registreringsfönstret
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _closeAction.Invoke();
        }
        // Valideringsmetod för användarnamn
        private bool ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Användarnamn kan inte vara tomt.";
                return false;
            }

            if (_userManager.IsUsernameTaken(Username))
            {
                ErrorMessage = "Användarnamnet är redan taget.";
                return false;
            }

            return true;
        }
        // Valideringsmetod för lösenord
        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password) || !ValidationHelper.IsValidPassword(Password))
            {
                ErrorMessage = "Lösenordet måste vara minst 8 tecken långt, innehålla minst en siffra och ett specialtecken.";
                return false;
            }

            return true;
        }
        // Valideringsmetod för bekräftelselösenord
        private bool ValidateConfirmPassword()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Lösenorden matchar inte.";
                return false;
            }

            return true;
        }
        // Valideringsmetod för valt land
        private bool ValidateCountry()
        {
            if (string.IsNullOrWhiteSpace(SelectedCountry))
            {
                ErrorMessage = "Vänligen välj ett land.";
                return false;
            }

            return true;
        }
        // Valideringsmetod för säkerhetsfråga och svar
        private bool ValidateSecurityQuestionAndAnswer()
        {
            if (string.IsNullOrWhiteSpace(SecurityQuestion))
            {
                ErrorMessage = "Vänligen ange en säkerhetsfråga.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(SecurityAnswer))
            {
                ErrorMessage = "Vänligen ange ett svar på säkerhetsfrågan.";
                return false;
            }

            return true;
        }

    }
}
