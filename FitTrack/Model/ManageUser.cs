using FitTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitTrack.Model
{
    public class ManageUser
    {
        // Singleton-instans av ManageUser för att säkerställa att det endast finns en hanterare
        private static ManageUser _instance;
        public static ManageUser Instance => _instance ??= new ManageUser();

        // Lista över registrerade användare
        public static ObservableCollection<User> RegisteredUsers { get; set; }

        // Egenskap för att hålla koll på nuvarande inloggade användare
        public User? LoggedInUser { get; private set; }

        // Standardadministratör som skapas när applikationen startar
        private AdminUser _defaultAdmin;

        private ManageUser()
        {
            // Skapa en standardadmin och två vanliga användare för teständamål
            _defaultAdmin = new AdminUser("Admin1", "Password1234!", "Sweden", "Fave Color?", "Blue");
            RegisteredUsers = new ObservableCollection<User>()
            {
                _defaultAdmin,
                new User("Anna", "Password123!", "Sweden", "Fave color?", "Green"),
                new User("User1", "Password456!", "Norway", "What's my name?", "Anna")
            };
        }
        // Hämtar alla träningspass för alla användare
        public IEnumerable<Workout> GetAllWorkouts()
        {
            return RegisteredUsers.SelectMany(user => user.UserWorkouts);
        }

        // Hämtar träningspass för en specifik användare baserat på användarnamn
        public IEnumerable<Workout> GetWorkoutsForUser(string username)
        {
            var user = GetUser(username);
            return user?.UserWorkouts ?? Enumerable.Empty<Workout>();
        }
        public bool ValidateCredentials(string username, string password)
        {
            var user = RegisteredUsers.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                LoggedInUser = user; // Sätter den nuvarande inloggade användaren
                return true;
            }
            LoggedInUser = null;
            return false;
        }
        // Lägger till en ny användare med validering av användarnamn och lösenord
        public bool AddUser(User user)
        {
            // Validera användarnamn
            if (!ValidationHelper.IsValidUsername(user.Username))
            {
                throw new ArgumentException("Användarnamnet måste vara minst 3 tecken långt.");
            }

            // Kontrollera om användarnamnet redan är taget
            if (IsUsernameTaken(user.Username))
            {
                throw new ArgumentException("Användarnamnet är redan taget.");
            }

            // Validera lösenord
            if (!ValidationHelper.IsValidPassword(user.Password))
            {
                throw new ArgumentException("Lösenordet måste vara minst 8 tecken, innehålla minst en siffra och ett specialtecken.");
            }

            RegisteredUsers.Add(user);
            return true;
        }
        // Tar bort en användare från listan över registrerade användare
        public void RemoveUser(User user)
        {
            RegisteredUsers.Remove(user);
        }

        // Hämtar en användare baserat på användarnamn
        public User? GetUser(string username)
        {
            return RegisteredUsers.FirstOrDefault(u => u.Username == username);
        }

        // Returnerar alla användare i listan
        public ObservableCollection<User> GetAllUsers()
        {
            return RegisteredUsers;
        }

        // Kontrollerar om ett användarnamn redan är taget
        public bool IsUsernameTaken(string username)
        {
            return RegisteredUsers.Any(u => u.Username == username);
        }
    }
}
