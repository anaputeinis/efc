using Android.App;
using Android.Widget;
using Android.OS;
using Firebase;
using Firebase.Auth;
using System;
using static Android.Views.View;
using Android.Views;
using Android.Gms.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace efc_final
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    
    public class MainActivity : Activity, IOnClickListener, IOnCompleteListener
    {
        private Button Main_Login;
        private EditText input_email, input_password;
        private TextView Main_SignUp, Main_ForgetPassword;
        private static TextView MainWarning;
        public static FirebaseApp AppFB;
        FirebaseAuth AuthFB;
        public static bool IsRunning=true;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Init Auth  
            InitFirebaseAuth();

            var user_id = (Preferences.Get("UserId", "-1"));
            if (user_id != "-1")
            {
                if (await SQLF.GetData_UserById(user_id))
                {
                    User.Current = JsonConvert.DeserializeObject<User>(SQLF.SQLF_answer_string);
                    AuthFB.SignInWithEmailAndPassword(User.Current.Email, User.Current.Password);
                    Preferences.Set("UserId", User.Current.Id.ToString());
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                    Finish();
                }
                else 
                {
                    SetContentView(Resource.Layout.main_layout);
                    //Views  
                    Main_Login = FindViewById<Button>(Resource.Id.login_btn_login);
                    input_email = FindViewById<EditText>(Resource.Id.login_email);
                    input_password = FindViewById<EditText>(Resource.Id.login_password);
                    Main_SignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
                    Main_ForgetPassword = FindViewById<TextView>(Resource.Id.login_btn_forget_password);
                    MainWarning = FindViewById<TextView>(Resource.Id.main_warning);

                    Main_SignUp.SetOnClickListener(this);
                    Main_Login.SetOnClickListener(this);
                    Main_ForgetPassword.SetOnClickListener(this);
                }
            }
            else
            {
                SetContentView(Resource.Layout.main_layout);
                //Views  
                Main_Login = FindViewById<Button>(Resource.Id.login_btn_login);
                input_email = FindViewById<EditText>(Resource.Id.login_email);
                input_password = FindViewById<EditText>(Resource.Id.login_password);
                Main_SignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
                Main_ForgetPassword = FindViewById<TextView>(Resource.Id.login_btn_forget_password);
                MainWarning = FindViewById<TextView>(Resource.Id.main_warning);

                Main_SignUp.SetOnClickListener(this);
                Main_Login.SetOnClickListener(this);
                Main_ForgetPassword.SetOnClickListener(this);
            }
        }

        // запрет нажатия на кнопку назад
        public override void OnBackPressed()
        {
            return;
        }

        // запрос разрешений
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        //инициализировать firebase 
        public void InitFirebaseAuth()
        {

            if (AppFB == null)
            {
                try
                {
                    var options = new FirebaseOptions.Builder()
                       .SetApplicationId("1:427494769742:android:4e118a73dedd5afcac2f1f")
                       .SetApiKey("AIzaSyAKvztquTS6_srVzb0LU_nKU0dVyAFIn_U")
                       .Build();
                    AppFB = FirebaseApp.InitializeApp(this, options);
                }
                catch (Exception)
                {
                    AppFB = FirebaseApp.GetInstance("[DEFAULT]");
                }

            }


            AuthFB = FirebaseAuth.GetInstance(AppFB);
        }

        // реакция на нажатие кнопок
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_forget_password)
            {
                StartActivity(new Android.Content.Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_login)
            {
                if (String.IsNullOrEmpty(input_email.Text) || String.IsNullOrEmpty(input_password.Text))
                {
                    MainWarning.Visibility = ViewStates.Visible;
                    MainWarning.Text = "Введите данные.";
                    return;
                }
                if (!IsRunning) { return;}
                LoginUser(input_email.Text, input_password.Text);

            }
        }

        // вход в аккаунт, параметры - пароль и адрес почты
        public void LoginUser(string email, string password)
        {
            AuthFB.SignInWithEmailAndPassword(email, password).AddOnCompleteListener(this);
        }
        private Dialog popupDialog;
        private Button btnPopupCancel;
        private TextView textView;

        public async void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                await SQLF.GetData_Id("UserTable", "firebase_uid", AuthFB.CurrentUser.Uid);
                var user_id = SQLF.SQLF_answer_string;
                if (await SQLF.GetData_UserById(user_id))
                {
                    User.Current = JsonConvert.DeserializeObject<User>(SQLF.SQLF_answer_string);
                    Preferences.Set("UserId", User.Current.Id.ToString());
                    if (User.Current.Password != input_password.Text) 
                    {
                        await SQLF.UpdateData_User(new List<string>() { User.Current.Id.ToString(), input_password.Text });
                    }
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                    Finish();
                }
                else 
                {
                    popupDialog = new Dialog(this);

                    popupDialog.SetContentView(Resource.Layout.warning_layout);
                    popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                    popupDialog.Show();

                    textView = popupDialog.FindViewById<TextView>(Resource.Id.warning_text);
                    btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.warning_ok_button);
                    textView.Text = "Вход не осуществлен.";
                    btnPopupCancel.Click += BtnPopupCancel_Click; 
                }
                
            }
            else
            {
                popupDialog = new Dialog(this);

                popupDialog.SetContentView(Resource.Layout.warning_layout);
                popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
                popupDialog.Show();

                textView = popupDialog.FindViewById<TextView>(Resource.Id.warning_text);
                btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.warning_ok_button);
                textView.Text = "Вход не осуществлен.";
                btnPopupCancel.Click += BtnPopupCancel_Click; 
            }
        }

        //скрытие информации о успешности входа в аккаунт
        private void BtnPopupCancel_Click(object sender, EventArgs e)
        {
            popupDialog.Dismiss();
            popupDialog.Hide();
        }
    }
}

