using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.Model;
using FitTrack.Commands;
using FitTrack.Helpers;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace FitTrack.ViewModel
{
    public class UserDetailsViewModel : BaseViewModel
    {
        private readonly User _user;
        private readonly ManageUser _userManager;

        public UserDetailsViewModel(User user, ManageUser userManager)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            Username = _user.Username;
            SelectedCountry = _user.Country ?? "DefaultCountry"; 
            Countries = CountryManager.GetCountries(); 
        }
        public string? Username { get; set; }
        public string? InputSecurityAnswer { get; set; }

        private string? _newPassword;
        public string? NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }
        private string? _confirmPassword;
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        public string? SelectedCountry { get; set; }

        // Meddelande som visas för användaren, t.ex. för fel eller framgång
        private string? _message;
        public string? Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        // Samling av länder som användaren kan välja mellan
        public ObservableCollection<string> Countries { get; set; }

        // Kommandon för att spara och avbryta ändringar
        public ICommand SaveCommand => new RelayCommand(_ => SaveUserDetails());
        public ICommand CancelCommand => new RelayCommand(_ => Cancel());
        public ICommand GoBackCommand => new RelayCommand(_ => GoBackToWorkouts());

        // Metod för att spara användaruppgifter med validering
        private void SaveUserDetails()
        {
            // Validera att användarnamnet är giltigt
            if (!ValidationHelper.IsValidUsername(Username))
            {
                Message = "Användarnamn måste vara minst 3 tecken.";
                return;
            }

            // Kontrollera om användarnamnet redan är upptaget
            if (_userManager.IsUsernameTaken(Username) && Username != _user.Username)
            {
                Message = "Användarnamnet är redan upptaget.";
                return;
            }
            // Validera lösenord om ett nytt lösenord anges
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                if (!ValidationHelper.IsValidPassword(NewPassword))
                {
                    Message = "Lösenordet måste vara minst 8 tecken, innehålla en stor bokstav, en siffra och ett specialtecken.";
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    Message = "Lösenorden matchar inte.";
                    return;
                }

                // Uppdaterar lösenord om det valideras korrekt
                _user.Password = NewPassword;
            }
            // Uppdatera användarens namn och land
            _user.Username = Username;
            _user.Country = SelectedCountry;
            Message = "Uppgifterna sparades!";
            GoBackToWorkouts(); // Gå tillbaka till WorkoutsWindow
        }
        private void Cancel()
        {
            Username = _user.Username;
            SelectedCountry = _user.Country;
            Message = "Ändringarna avbröts.";
            GoBackToWorkouts(); // Gå tillbaka till WorkoutsWindow
        }
        // Metod för att navigera tillbaka till WorkoutsWindow
        private void GoBackToWorkouts()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }


    }
} 
