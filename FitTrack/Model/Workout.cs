﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack.Model
{
    public abstract class Workout
    {
        // Egenskaper för träningspassets datum, typ, varaktighet, brända kalorier och anteckningar
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public TimeSpan Duration { get; set; }
        public int CaloriesBurned { get; set; }
        public string Notes { get; set; }
        public string Owner { get; set; }  // Ägaren till träningspasset(användarnamn)

        //Konstruktor
        public Workout(DateTime date, string type, TimeSpan duration, string notes, string owner)
        {
            Date = date;
            Type = type;
            Duration = duration;
            Notes = notes;
            Owner = owner;
        }
        public virtual int CalculateCaloriesBurned()
        {
            return CaloriesBurned;
        }
        public void UpdateCaloriesBurned(int calories)
        {
            CaloriesBurned = calories;
        }
    }
}
