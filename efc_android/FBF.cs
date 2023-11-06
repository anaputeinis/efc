using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace efc_final
{
    public class FBF
    {
       
        private const string FirebaseURL = "https://efcfinal-default-rtdb.europe-west1.firebasedatabase.app/";
        private static FirebaseClient firebase = new FirebaseClient(FirebaseURL);
        static public async void CreateUser(User user)
        {
            var jsonString = JsonConvert.SerializeObject(user);
            await firebase.Child("users" + "/" + user.FirebaseUID).PostAsync(jsonString);
        }
        static public async void UpdateUser(User user)
        {
            var jsonString = JsonConvert.SerializeObject(user);
            await firebase.Child("users" + "/" + user.FirebaseUID).PutAsync(jsonString);

        }
        static public async void DeleteUser(User user)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users" + "/" + user.FirebaseUID).DeleteAsync();
        }
    }
}