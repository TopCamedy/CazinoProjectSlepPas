using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CazinoProjectSlepPas
{
    public class User : INotifyPropertyChanged
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Balance { get; set; } = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropChange([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        Int128 nowBalance = 0;

        public Int128 NowBalance
        {
            get => nowBalance;
            set
            {
                nowBalance = value;
                OnPropChange();
            }
        }

    }
    public static class UserDatabase 
    {
        public static Dictionary<string, User> _users = new Dictionary<string, User>();


        


        public static bool AddUser(string login, string password, DateTime birthDate)
        {
            if (_users.ContainsKey(login))
                return false;

            _users.Add(login, new User
            {
                Login = login,
                Password = password,
                BirthDate = birthDate
            });

            

            return true;
        }

        public static bool UserExists(string login)
        {
            return _users.ContainsKey(login);
        }

        public static bool CheckPassword(string login, string password)
        {
            return _users.TryGetValue(login, out var user) && user.Password == password;
        }

        public static User GetUser(string login)
        {
            _users.TryGetValue(login, out var user);
            return user;
        }

        public static bool UpdateBalance(string login, decimal amount)
        {
            if (!_users.TryGetValue(login, out var user))
                return false;

            user.Balance += amount;
            return true;
        }

    }
}