using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack.Model
{
    public class StrengthWorkout:Workout
    {
        // Egenskaper för repetitions och set
        public int Reps { get; set; }
        public int Sets { get; set; }

        private Func<int, int, int> calculateCalories = (reps, sets) => (reps * sets) * 5;

        // Konstruktor som sätter egenskaper och beräknar kalorier
        public StrengthWorkout(DateTime date, string type, TimeSpan duration, string notes, int reps, int sets, string owner)
            : base(date, type, duration, notes, owner)
        {
            Reps = reps;
            Sets = sets;
            CaloriesBurned = calculateCalories(reps, sets); 
        }
        public override int CalculateCaloriesBurned()
        {
            return CaloriesBurned;
        }
    }
}
