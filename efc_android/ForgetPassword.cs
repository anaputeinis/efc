using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using static Android.Views.View;
namespace efc_final
{
    [Activity(Label = "ForgetPassword", Theme = "@style/AppTheme")]
    public class ForgetPassword : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        EditText input_email;
        Button ResetPassword_Button;
        TextView Back_Button, ForgetPassword_Warning;
        FirebaseAuth auth;

        Dialog popupDialog;
        Button btnPopupCancel;
        TextView textView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.forget_password_layout);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.AppFB);

            //Views  
            input_email = FindViewById<EditText>(Resource.Id.forget_email);
            ResetPassword_Button = FindViewById<Button>(Resource.Id.forget_btn_reset);
            Back_Button = FindViewById<TextView>(Resource.Id.forget_btn_back);
            ForgetPassword_Warning = FindViewById<TextView>(Resource.Id.forget_password_warning);

            ResetPassword_Button.SetOnClickListener(this);
            Back_Button.SetOnClickListener(this);
        }

        //нажатие на кнопки запускает процесс сброса пароля
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.forget_btn_back)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.forget_btn_reset)
            {
                if (String.IsNullOrEmpty(input_email.Text))
                {
                    ForgetPassword_Warning.Visibility = ViewStates.Visible;
                    ForgetPassword_Warning.Text = "Введите данные.";
                    return;
                }
                ResetPassword(input_email.Text);
            }
        }

        //запрет нажатия на кнопку "назад"
        public override void OnBackPressed()
        {
            return;
        }

        // смена пароля
        private void ResetPassword(string email)
        {
            auth.SendPasswordResetEmail(email).AddOnCompleteListener(this, this);
        }


        // выполненяется после завершения сброса пароля firebase, сообщает об успешности операции
        public void OnComplete(Task task)
        {
            popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.warning_layout);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog.Show();

            btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.warning_ok_button);
            textView = popupDialog.FindViewById<TextView>(Resource.Id.warning_text);

            btnPopupCancel.Click += BtnPopupCancel_Click;
            if (!task.IsSuccessful)
            {
                textView.Text = "Reset Password Failed!";
            }
            else
            {
                textView.Text = "Reset Password link send to email : " + input_email.Text;
            }
        }

        // скрытие информации о статусе смены пароля
        private void BtnPopupCancel_Click(object sender, EventArgs e)
        {
            popupDialog.Dismiss();
            popupDialog.Hide();
        }
    }


}