using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitTrack.Commands;
using FitTrack.Model;
using System.ComponentModel;
using System.Windows.Input;

namespace FitTrack.ViewModel
{
    public class AddWorkoutViewModel : BaseViewModel
    {
        // Egenskaper 
        public DateTime WorkoutDate { get; set; } = DateTime.Today;
        public string WorkoutType { get; set; }
        public TimeSpan WorkoutDuration { get; set; }
        public int WorkoutCaloriesBurned { get; set; }
        public string WorkoutNotes { get; set; }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action WorkoutSaved;
        public event Action Cancelled;

        public AddWorkoutViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveWorkout(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }
        // Validering: Kontrollerar om alla fält är korrekt ifyllda
        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(WorkoutType) &&
                   WorkoutDuration > TimeSpan.Zero &&
                   WorkoutCaloriesBurned > 0 &&
                   !string.IsNullOrWhiteSpace(WorkoutNotes);
        }
        private void SaveWorkout()
        {
            
            var newWorkout = new UserWorkoutInfo(
                username: "Username",
                workoutType: WorkoutType,
                duration: WorkoutDuration
            );

            // Anropa event för att meddela att träningspasset har sparats
            WorkoutSaved?.Invoke();
        }
        private void Cancel()
        {
            Cancelled?.Invoke(); 
        }
    }
}
