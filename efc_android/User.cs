using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace efc_final
{
    // класс, отвечающий за аккаунт
    public class User
    {
        public static User Current;

        public int Id { get; set; }

        public string FirebaseUID { get; set; }

        public List<AllergyProfile> AllergyProfileList { get; set; }

        public List<Item> FavouriteItems { get; set; }

        public List<Item> History { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public User() { }

        public User(string uid, string email, string password)
        {
            FirebaseUID = uid;
            AllergyProfileList = new List<AllergyProfile>();
            FavouriteItems = new List<Item>();
            History = new List<Item>();
            Email = email;
            Password = password;
        }

        static User()
        {
            Current = new User();
        }

        // добавление нового профиля, параметр - добавляемый профиль
        public void AddAlllergyProfile(AllergyProfile profile) 
        {
            this.AllergyProfileList.Add(profile);
        }
    }
}