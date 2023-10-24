using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using static Android.Views.View;
using Task = Android.Gms.Tasks.Task;

namespace efc_final
{
    [Activity(Label = "SignUp", Theme = "@style/AppTheme")]
    public class SignUp : Activity, IOnClickListener, IOnCompleteListener
    {
        Button SignUp_RegisterButton, SignUp_PopUpCancelButton;
        TextView SignUp_AlreadyHaveAnAccount, SignUp_ForgetPassword, SignUpWarning,SignUp_PopUpTextView;
        EditText SignUp_Email, SignUp_Password;
        FirebaseAuth auth;
        Dialog SignUp_PopupDialog;
        string password, email;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sign_up_layout);

            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);

            //Views  
            SignUp_RegisterButton = FindViewById<Button>(Resource.Id.signup_btn_register);
            SignUp_AlreadyHaveAnAccount = FindViewById<TextView>(Resource.Id.signup_btn_login);
            SignUp_ForgetPassword = FindViewById<TextView>(Resource.Id.signup_btn_forget_password);
            SignUp_Email = FindViewById<EditText>(Resource.Id.signup_email);
            SignUp_Password = FindViewById<EditText>(Resource.Id.signup_password);
            SignUpWarning = FindViewById<TextView>(Resource.Id.sign_up_warning);

            //Buttons
            SignUp_AlreadyHaveAnAccount.SetOnClickListener(this);
            SignUp_RegisterButton.SetOnClickListener(this);
            SignUp_ForgetPassword.SetOnClickListener(this);

        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_forget_password)
            {
                StartActivity(new Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_register)
            {
                if (String.IsNullOrEmpty(SignUp_Email.Text) || String.IsNullOrEmpty(SignUp_Password.Text))
                {
                    SignUpWarning.Visibility = ViewStates.Visible;
                    SignUpWarning.Text = "Введите данные.";
                    return;
                }
                email = SignUp_Email.Text;
                password = SignUp_Password.Text;
                SignUpUser();
            }
        }

        // создание пользователя с помощью firebase
        private void SignUpUser()
        {
            auth.CreateUserWithEmailAndPassword(email, password).AddOnCompleteListener(this, this);
        }
        public async void OnComplete(Task task)
        {
            SignUp_PopupDialog = new Dialog(this);
            SignUp_PopupDialog.SetContentView(Resource.Layout.warning_layout);
            SignUp_PopupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            SignUp_PopupDialog.Show();

            SignUp_PopUpCancelButton = SignUp_PopupDialog.FindViewById<Button>(Resource.Id.warning_ok_button);
            SignUp_PopUpTextView = SignUp_PopupDialog.FindViewById<TextView>(Resource.Id.warning_text);

            SignUp_PopUpCancelButton.Click += SignUp_PopUpCancelButton_Click;
            if (task.IsSuccessful)
            {
                if (await SignInUser_New()) 
                {
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                }
                else
                {
                    SignUp_PopUpTextView.Text = "Войти не получилось.";
                }
            }
            else
            {
                SignUp_PopUpTextView.Text = "Зарегистрироваться не получилось.";
            }
        }

        // создание пользователя на SQL сервере
        private async Task<bool> SignInUser_New() 
        {
            bool c = true;
            try
            {
                auth.SignInWithEmailAndPassword(email, password);
                string uid = auth.CurrentUser.Uid;

                if (!await SQLF.UploadData_NewUser(uid)) { c = false; }
                else 
                { 
                    int id = Int32.Parse(SQLF.SQLF_answer_string);
                    User.Current = new User(uid);
                    User.Current.Id = id;
                }  
            }
            catch 
            {
                c = false;
            }

            return c;
        }
        // скрытие информации о статусе регистрации
        private void SignUp_PopUpCancelButton_Click(object sender, EventArgs e)
        {
            SignUp_PopupDialog.Dismiss();
            SignUp_PopupDialog.Hide();
        }
        // скрытие текста предупреждения
        private void Clear(object sender, EventArgs e)
        {
            SignUpWarning.Visibility = ViewStates.Gone;
        }



    }
}