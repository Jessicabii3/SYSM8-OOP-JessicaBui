using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.Model;
using FitTrack.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using FitTrack.View;
using System.Windows;


namespace FitTrack.ViewModel
{
    public class WorkoutDetailsViewModel:BaseViewModel
    {
        // Fält för att hålla redigeringsstatus
        private bool _isEditing;
        private readonly Workout _workout;
        private Workout _originalWorkout;

        // Lista över alla träningspass, används för att lägga till/ta bort pass
        private ObservableCollection<Workout> _workouts;

        // Hanterare för användarinformation och behörigheter
        private ManageUser _userManager;

        // Konstruktor som tar träningspasset, listan över träningspass och användarhanteraren
        public WorkoutDetailsViewModel(Workout workout, ObservableCollection<Workout> workouts, ManageUser userManager)
        {
            _workout = workout ?? throw new ArgumentNullException(nameof(workout));
            _originalWorkout = CloneWorkout(workout); // Skapa en kopia av workout för återställning
            _workouts = workouts ?? throw new ArgumentNullException(nameof(workouts));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            // Initialisering av kommandon
            EditCommand = new RelayCommand(_ => EnableEditing());
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => CancelEditing());
            CopyCommand = new RelayCommand(_ => CopyWorkout());
            GoBackCommand = new RelayCommand(_ => GoBack());
            DeleteCommand = new RelayCommand(_ => DeleteWorkout());

            IsEditing = false; // Initialt är redigering inaktiverad

        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // Uppdaterar kommandon beroende på redigeringsläge
            }
        }
        // Egenskap för att kontrollera om användaren är admin
        public bool IsAdmin => _userManager.LoggedInUser?.IsAdmin ?? false;

        // Datum för träningspasset, som visas i gränssnittet
        public string WorkoutDate
        {
            get => _workout.Date.ToString("yyyy-MM-dd");
            set
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    _workout.Date = date;
                    OnPropertyChanged();
                }
            }
        }
        public string WorkoutType
        {
            get => _workout.Type;
            set
            {
                _workout.Type = value;
                OnPropertyChanged();
            }
        }
        // Varaktighet för träningspasset
        public string WorkoutDuration
        {
            get => _workout.Duration.ToString(@"hh\:mm");
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan duration))
                {
                    _workout.Duration = duration;
                    OnPropertyChanged();
                }
            }
        }
        // Kaloriförbränning för träningspasset
        public int WorkoutCaloriesBurned
        {
            get => _workout.CaloriesBurned;
            set
            {
                _workout.CaloriesBurned = value;
                OnPropertyChanged();
            }
        }
        // Anteckningar om träningspasset
        public string WorkoutNotes
        {
            get => _workout.Notes;
            set
            {
                _workout.Notes = value;
                OnPropertyChanged();
            }
        }

        public bool CanDeleteWorkout => IsAdmin || _workout.Owner == _userManager.LoggedInUser?.Username;
        // Kommandon för olika åtgärder
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand DeleteCommand { get; }

        // Metod för att aktivera redigeringsläge
        private void EnableEditing() => IsEditing = true;

        private void SaveWorkout()
        {
            IsEditing = false; // Avsluta redigeringsläge
            GoBack();
        }
        // Kontroll om det är möjligt att spara ändringar
        private bool CanSave()
        {
            if (!IsEditing) return false;

            // Kontrollera att datum inte ligger i framtiden
            if (DateTime.TryParse(WorkoutDate, out DateTime date))
            {
                if (date > DateTime.Today)
                    return false; // Ogiltigt om datumet är i framtiden
            }
            else
            {
                return false; // Ogiltigt om datumet inte kan tolkas korrekt
            }

            // Kontrollera att typ av träningspass är giltigt
            if (string.IsNullOrWhiteSpace(WorkoutType))
                return false;

            // Kontrollera att varaktighet är satt och giltig
            if (!TimeSpan.TryParse(WorkoutDuration, out TimeSpan duration) || duration <= TimeSpan.Zero)
                return false;

            // Kontrollera att kaloriförbränning är större än noll
            if (WorkoutCaloriesBurned <= 0)
                return false;

            // Om alla kontroller har klarat, returnera true
            return true;
        }
            

        // Metod för att avbryta redigering och återställa till ursprungsvärdena
        private void CancelEditing()
        {
            RestoreWorkout(_workout, _originalWorkout); // Återställer ändringar
            IsEditing = false; // Avsluta redigeringsläge
            OnPropertyChanged(string.Empty); 
        }
        // Metod för att kopiera träningspasset till listan över träningspass
        private void CopyWorkout()
        {
            Workout newWorkout = CloneWorkout(_workout); // Skapar en kopia av workout
            _workouts.Add(newWorkout); // Lägg till kopian i träningslistan
        }
        // Metod för att ta bort träningspasset från listan
        private void DeleteWorkout()
        {
            _workouts.Remove(_workout); // Ta bort workout från listan
            GoBack(); 
        }
        // Metod för att stänga WorkoutDetailsWindow och öppna WorkoutsWindow
        private void GoBack()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close(); // Stänger nuvarande fönster

            // Öppnar WorkoutsWindow
            var workoutsViewModel = new WorkoutsViewModel(_userManager);
            var workoutsWindow = new WorkoutsWindow(workoutsViewModel)
            {
                DataContext = workoutsViewModel
            };
            workoutsWindow.Show();
        }
        // Metod för att klona (kopiera) ett träningspass
        private Workout CloneWorkout(Workout workout)
        {
            return workout switch
            {
                CardioWorkout cardio => new CardioWorkout(cardio.Date, cardio.Duration, cardio.Distance, cardio.Notes, cardio.Owner),
                StrengthWorkout strength => new StrengthWorkout(strength.Date, strength.Type, strength.Duration, strength.Notes, strength.Reps, strength.Sets, strength.Owner),
                _ => throw new InvalidOperationException("Okänd träningspass typ") // Fel om träningspass-typen är okänd
            };
        }
        // Metod för att återställa träningspasset till dess ursprungliga värden
        private void RestoreWorkout(Workout target, Workout source)
        {
            target.Date = source.Date;
            target.Type = source.Type;
            target.Duration = source.Duration;
            target.Notes = source.Notes;
        }
    }
}
