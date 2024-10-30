using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.View;
using FitTrack.Model;
using FitTrack.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using FitTrack.ViewModel;

namespace FitTrack.ViewModel
{
    public class WorkoutsViewModel:BaseViewModel
    {
        public ObservableCollection<Workout> Workouts { get; private set; }
        private Workout? _selectedWorkout;

        // Hanterare för användardata och behörigheter
        private ManageUser _userManager;

        private string _errorMessage;

        // Filteregenskaper för att filtrera träningspass
        private DateTime? _filterDate; 
        private string? _filterType; 
        private TimeSpan? _filterDuration;

        // Egenskap för att kontrollera om den inloggade användaren är admin
        public bool IsAdmin => _userManager.LoggedInUser?.IsAdmin ?? false;

        // Egenskap för att visa användarnamn i UI
        public string Username => _userManager.LoggedInUser?.Username ?? string.Empty;
        // Egenskap för det aktuella valda träningspasset
        public Workout? SelectedWorkout
        {
            get => _selectedWorkout;
            set
            {
                _selectedWorkout = value;
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
        // Filteregenskaper med OnPropertyChanged för att automatiskt tillämpa filter när de ändras
        public DateTime? FilterDate
        {
            get => _filterDate;
            set
            {
                _filterDate = value;
                OnPropertyChanged();
                ApplyFilters(); 
            }
        }
        public string FilterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                OnPropertyChanged();
                ApplyFilters(); 
            }
        }
        public TimeSpan? FilterDuration
        {
            get => _filterDuration;
            set
            {
                _filterDuration = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        // Kommandon 
        public RelayCommand AddWorkoutCommand { get; }
        public RelayCommand DeleteWorkoutCommand { get; }
        public RelayCommand OpenUserDetailsCommand { get; }
        public RelayCommand OpenWorkoutDetailsCommand { get; }
        public RelayCommand LogOutCommand { get; }

        // Konstruktor som initierar egenskaper och laddar träningspass baserat på användarbehörigheter
        public WorkoutsViewModel(ManageUser manageUser)
        {
            _userManager = manageUser ?? throw new ArgumentNullException(nameof(manageUser)); // Säkerställer att ManageUser inte är null
            Workouts = new ObservableCollection<Workout>();
            LoadWorkouts(); // Laddar träningspass baserat på användarroll (admin eller vanlig användare)

            // Initialiserar kommandon med tillhörande metoder
            AddWorkoutCommand = new RelayCommand(_ => AddWorkout(),_=> !IsAdmin);
            DeleteWorkoutCommand = new RelayCommand(_ => DeleteWorkout(), _ => CanDeleteWorkout());
            OpenUserDetailsCommand = new RelayCommand(_ => OpenUserDetails());
            OpenWorkoutDetailsCommand = new RelayCommand(_ => OpenWorkoutDetails());
            LogOutCommand = new RelayCommand(_ => LogOut());
        }
        private void LoadWorkouts()
        {
            var workouts = _userManager.LoggedInUser.IsAdmin
                ? _userManager.GetAllWorkouts() // Admin ser alla träningspass
                : _userManager.GetAllWorkouts().Where(w => w.Owner == _userManager.LoggedInUser.Username); // Vanliga användare ser endast sina träningspass

            Workouts = new ObservableCollection<Workout>(workouts);
        }
        public void AddWorkout()
        {
            var newWorkout = new CardioWorkout(DateTime.Now, TimeSpan.FromMinutes(30), 5, "Morgonlöpning", _userManager.LoggedInUser.Username);
            Workouts.Add(newWorkout);
        }
        // Öppnar WorkoutDetailsWindow med detaljer för det valda träningspasset
        private void OpenWorkoutDetails()
        {
            if (SelectedWorkout != null)
            {
                var workoutDetailsViewModel = new WorkoutDetailsViewModel(SelectedWorkout, Workouts, _userManager);
                var workoutDetailsWindow = new WorkoutDetailsWindow(workoutDetailsViewModel);
                workoutDetailsWindow.Show();
            }
            else
            {
                ErrorMessage = "Välj ett träningspass för att se detaljer."; // Informerar användaren om att välja ett träningspass
            }
        }
        // Raderar det valda träningspasset om det är satt, annars visas ett felmeddelande
        private void DeleteWorkout()
        {
            if (SelectedWorkout != null && CanDeleteWorkout())
            {
                Workouts.Remove(SelectedWorkout);
                SelectedWorkout = null; // Tömmer valet efter radering
            }
            else
            {
                ErrorMessage = "Välj ett träningspass att radera."; // Felmeddelande om inget träningspass är valt
            }
        }
        private bool CanDeleteWorkout() => SelectedWorkout != null;

        // Öppnar UserDetailsWindow för att redigera användarens uppgifter
        private void OpenUserDetails()
        {
            var user = _userManager.LoggedInUser;
            if (user != null)
            {
                var userDetailsWindow = new UserDetailsWindow(new UserDetailsViewModel(user, _userManager));
                userDetailsWindow.Show();
            }
        }
        private void LogOut()
        {
            Application.Current.MainWindow.Close();
        }
        // Tillämpa filter på listan med träningspass baserat på datum, typ och varaktighet
        private void ApplyFilters()
        {
            // Börjar med alla träningspass
            var filteredWorkouts = _userManager.GetAllWorkouts();

            // Tillämpa datumfilter om det är satt
            if (FilterDate.HasValue)
                filteredWorkouts = filteredWorkouts.Where(w => w.Date.Date == FilterDate.Value.Date);

            // Tillämpa typfilter om det är satt och inte tomt
            if (!string.IsNullOrWhiteSpace(FilterType))
                filteredWorkouts = filteredWorkouts.Where(w => w.Type.Equals(FilterType, StringComparison.OrdinalIgnoreCase));

            // Tillämpa varaktighetsfilter om det är satt
            if (FilterDuration.HasValue)
                filteredWorkouts = filteredWorkouts.Where(w => w.Duration >= FilterDuration.Value);

            // Uppdaterar ObservableCollection för att visa filtrerade resultat
            Workouts = new ObservableCollection<Workout>(filteredWorkouts);
            OnPropertyChanged(nameof(Workouts)); // Meddelar UI om att listan har uppdaterats
        }

    }
}
