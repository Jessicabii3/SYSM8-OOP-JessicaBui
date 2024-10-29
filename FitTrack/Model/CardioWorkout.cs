using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack.Model
{
    public class CardioWorkout: Workout
    {
        //Egenskap
        public int Distance { get; set; }
        //Konstruktor
        public CardioWorkout(DateTime date, TimeSpan duration, int distance, string notes, string owner)
           : base(date, "Cardio", duration, notes, owner)
        {
            Distance = distance;
        }
        public override int CalculateCaloriesBurned()
        {
            // Beräkning av kalorier baserat på distans och tid
            CaloriesBurned = (int)(Distance * 0.75 * Duration.TotalMinutes);
            return CaloriesBurned;
        }
    }
}
