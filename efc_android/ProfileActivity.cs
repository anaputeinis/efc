using Android.App;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Intent = Android.Content.Intent;
using Task = Android.Gms.Tasks.Task;

namespace efc_final
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : Activity, IOnCompleteListener
    {
        AllergyProfile Edited_AllergyProfile, Created_AllergyProfile, Original_AllergyProfile;
        List<string> Edited_AllergyItem;
        private EditText Create_NameEditable, Create_AddAllergy, Edit_AddAllergy, FirstPassword, SecondPassword;
        private TextView Edit_Warning, Create_Warning, Instruction, Edit_Name, Create_Name, First_Instruction;
        private ListView CreateAllergies_ListView, EditAllergies_ListView, ProfileActivity_ListView;
        private Dialog popupDialog_edit_profile, popupDialog_create_profile;
        private Button Edit_AddAllergyButton, Edit_SaveButton, Edit_YesButton, Edit_NoButton, Edit_DeleteButton,
            Create_SaveButton, Create_YesButton, Create_NoButton, Create_NameButton, Create_AddAllergyButton, Create_DeleteButton,
            Password_ChangeButton, Password_SaveButton, Logout_Button;
        FirebaseAuth auth;

        private Dialog popupDialog;
        private Button btnPopupCancel;
        private TextView textView;
        private AdapterProfiles profile_activity_adapter;
        private AdapterPlain edit_adapter;
        private AdapterPlain create_adapter;
        private ImageButton FavouritesButton, HistoryButton;

        private CheckBox Create_Box_gluten, Create_Box_lactose, Edit_Box_gluten, Edit_Box_lactose,First_G,First_L;

        protected async override void OnCreate(Bundle savedInstanceState)

        {

            base.OnCreate(savedInstanceState);
            auth = FirebaseAuth.GetInstance(MainActivity.AppFB);

            SetContentView(Resource.Layout.profile_layout);

            ProfileActivity_ListView = FindViewById<ListView>(Resource.Id.profile_listview);
            if (User.Current.AllergyProfileList.Count == 0)
            {
                if (await SQLF.GetData_UserProfiles(User.Current.Id.ToString()))
                {
                    User.Current.AllergyProfileList = JsonConvert.DeserializeObject<List<AllergyProfile>>(SQLF.SQLF_answer_string);
                }
            }
            profile_activity_adapter = new AdapterProfiles(this, User.Current.AllergyProfileList);
            ProfileActivity_ListView.Adapter = profile_activity_adapter;

            Button Create_AllergyProfileButton = FindViewById<Button>(Resource.Id.add_allergy_profile_button);
            Logout_Button = FindViewById<Button>(Resource.Id.profile_logout_button);
            Password_ChangeButton = FindViewById<Button>(Resource.Id.profile_change_password_button);
            Password_SaveButton = FindViewById<Button>(Resource.Id.profile_save_button);
            FirstPassword = FindViewById<EditText>(Resource.Id.profile_first_password);
            SecondPassword = FindViewById<EditText>(Resource.Id.profile_second_password);
            FavouritesButton = FindViewById<ImageButton>(Resource.Id.imageButton_fav);
            HistoryButton = FindViewById<ImageButton>(Resource.Id.imageButton_his);

            FavouritesButton.SetImageResource(Resource.Drawable.love_pc);
            HistoryButton.SetImageResource(Resource.Drawable.history_pc);

            FavouritesButton.Click += FavouritesButton_Click;
            ProfileActivity_ListView.ItemLongClick += ProfileActivityListView_ItemLongClick;
            Create_AllergyProfileButton.Click += Create_AllergyProfileButton_Click;
            Password_ChangeButton.Click += Password_Change_Click;
            Logout_Button.Click += Logout_Button_Click;
            Password_SaveButton.Click += Password_SaveButton_Click;
            HistoryButton.Click += HistoryButton_Click;
        }

        // просмотр истории
        private void HistoryButton_Click(object sender, EventArgs e)
        {
            HistoryPage.HistoryList = new List<Item>();
            StartActivity(new Intent(this, typeof(HistoryPage)));

        }

        // просмотр раздела любимое
        private void FavouritesButton_Click(object sender, EventArgs e)
        {
            FavouritesPage.FavouritesList = new List<Item>();
            StartActivity(new Intent(this, typeof(FavouritesPage)));
        }

        // выход из аккаунта
        private void Logout_Button_Click(object sender, EventArgs e)
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                Preferences.Clear();
                StartActivity(new Android.Content.Intent(this, typeof(MainActivity)));
                Finish();
            }
        }

        // смена пароля, появление полей для ввода нового пароля
        private void Password_Change_Click(object sender, EventArgs e)
        {
            Password_SaveButton.Visibility=ViewStates.Visible;
            FirstPassword.Visibility=ViewStates.Visible;
            SecondPassword.Visibility=ViewStates.Visible;
        }

        // сохранение нового пароля
        private void Password_SaveButton_Click(object sender, EventArgs e)
        {
            if (FirstPassword.Text == SecondPassword.Text && !String.IsNullOrWhiteSpace(FirstPassword.Text)) 
            {
                
                Password_SaveButton.Visibility = ViewStates.Gone;
                FirstPassword.Visibility = ViewStates.Gone;
                SecondPassword.Visibility = ViewStates.Gone;
           
                FirebaseUser user = auth.CurrentUser;
                user.UpdatePassword(FirstPassword.Text).AddOnCompleteListener(this);
            }
        }
        public async void OnComplete(Task task)
        {
            //MainActivity.PasswordChanger_Check = 1;
            popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.warning_layout);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog.Show();

            btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.warning_ok_button);
            textView = popupDialog.FindViewById<TextView>(Resource.Id.warning_text);

            btnPopupCancel.Click += BtnPopupCancel_Click;
            if (task.IsSuccessful)
            {
                auth.SignOut();
                auth.SignInWithEmailAndPassword(User.Current.Email, FirstPassword.Text);
                await SQLF.UpdateData_User(new List<string>() { User.Current.Id.ToString(), FirstPassword.Text });
                textView.Text = "Пароль успешно изменен";
                return;
                
            }
            textView.Text = "Пароль сменить не удалось. Пожалуйста, перезайдите в приложение и попробуйте еще раз.";
        }


        // скрытие сообщения об успешности смены пароля
        private void BtnPopupCancel_Click(object sender, EventArgs e)
        {
            
            FirstPassword.Text= string.Empty;
            SecondPassword.Text= string.Empty;
            textView.Text = string.Empty;
            popupDialog.Dismiss();
            popupDialog.Hide();
        }


        //CREATE PROFILE ACTION

        // выбор опции добавление нового профиля
        private void Create_AllergyProfileButton_Click(object sender, EventArgs e)
        {
            popupDialog_create_profile = new Dialog(this);
            popupDialog_create_profile.SetContentView(Resource.Layout.profile_popup);
            popupDialog_create_profile.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog_create_profile.Show();

            Create_NameEditable = popupDialog_create_profile.FindViewById<EditText>(Resource.Id.profile_popup_name_text_edit);
            Create_Name= popupDialog_create_profile.FindViewById<TextView>(Resource.Id.profile_popup_name_text);
            Create_NameButton = popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_new_allergy_profile_button);
            Create_NameEditable.Visibility = ViewStates.Visible;
            Create_NameButton.Visibility=ViewStates.Visible;

            CreateAllergies_ListView = popupDialog_create_profile.FindViewById<ListView>(Resource.Id.profile_popup_listview);
            Create_SaveButton = popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_save_button);
            Create_Warning = popupDialog_create_profile.FindViewById<TextView>(Resource.Id.profile_popup_warning);
            Create_YesButton = popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_yes_button);
            Create_NoButton = popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_no_button);
            Create_AddAllergy= popupDialog_create_profile.FindViewById<EditText>(Resource.Id.profile_popup_add_text);
            Create_AddAllergyButton= popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_add_allergy_button);
            Create_DeleteButton = popupDialog_create_profile.FindViewById<Button>(Resource.Id.profile_popup_delete_button);
            Create_Box_gluten = popupDialog_create_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_gluten);
            Create_Box_lactose = popupDialog_create_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_lactose);
            Instruction = popupDialog_create_profile.FindViewById<TextView>(Resource.Id.profile_checkbox_instruction);

            First_Instruction = popupDialog_create_profile.FindViewById<TextView>(Resource.Id.profile_checkbox_instruction_name);
            First_G = popupDialog_create_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_gluten_name);
            First_L = popupDialog_create_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_lactose_name);
            


            CreateAllergies_ListView.Visibility = ViewStates.Gone;
            Create_SaveButton.Visibility = ViewStates.Gone;
            Create_AddAllergy.Visibility = ViewStates.Gone;
            Create_AddAllergyButton.Visibility = ViewStates.Gone;
            Create_DeleteButton.Visibility = ViewStates.Gone;
            Create_Box_gluten.Visibility = ViewStates.Gone;
            Create_Box_lactose.Visibility = ViewStates.Gone;
            Instruction.Visibility=ViewStates.Gone;
            Create_Name.Visibility=ViewStates.Gone;

            First_G.Visibility = ViewStates.Visible;
            First_L.Visibility = ViewStates.Visible;



            Create_NameButton.Click += Create_NameButton_Click;
            Create_AddAllergyButton.Click += Create_AddAllergyButton_Click;
            Create_SaveButton.Click += Create_SaveButton_Click;
            Create_DeleteButton.Click += Create_DeleteButton_Click;
            CreateAllergies_ListView.ItemLongClick += Create_Allergies_ListView_ItemLongClick;
        }

        // отмена добавления нового профиля
        private void Create_DeleteButton_Click(object sender, EventArgs e)
        {
            popupDialog_create_profile.Dismiss();
            popupDialog_create_profile.Hide();
        }

        // удаление ингредиента из списка аллергий профиля
        private void Create_Allergies_ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Edited_AllergyItem = Created_AllergyProfile.AllergyList[e.Position];
            Create_Warning.Text = "Вы уверены, что хотите удалить данный аллерген?";

            Create_Warning.Visibility = ViewStates.Visible;
            Create_YesButton.Visibility = ViewStates.Visible;
            Create_NoButton.Visibility = ViewStates.Visible;
            Create_AddAllergyButton.Visibility = ViewStates.Visible;

            Create_AddAllergy.Visibility = ViewStates.Gone;
            Create_AddAllergyButton.Visibility= ViewStates.Gone;
            Instruction.Visibility=ViewStates.Gone;
            Create_Box_gluten.Visibility= ViewStates.Gone;
            Create_Box_lactose.Visibility= ViewStates.Gone;


            Create_YesButton.Click += Create_YesButton_Click;
            Create_NoButton.Click += Create_NoButton_Click;
        }

        // удаление ингредиента из списка аллергий профиля - выбор варианта нет
        private void Create_NoButton_Click(object sender, EventArgs e)
        {
            Create_Warning.Visibility = ViewStates.Gone;
            Create_YesButton.Visibility = ViewStates.Gone;
            Create_NoButton.Visibility = ViewStates.Gone;

            Instruction.Visibility = ViewStates.Visible;
            Create_Box_gluten.Visibility = ViewStates.Visible;
            Create_Box_lactose.Visibility = ViewStates.Visible;
            Create_AddAllergy.Visibility = ViewStates.Visible;
            Create_AddAllergyButton.Visibility = ViewStates.Visible;
        }

        // удаление ингредиента из списка аллергий профиля - выбор варианта да
        private void Create_YesButton_Click(object sender, EventArgs e)
        {
            Create_Warning.Visibility = ViewStates.Gone;
            Create_YesButton.Visibility = ViewStates.Gone;
            Create_NoButton.Visibility = ViewStates.Gone;

            Instruction.Visibility = ViewStates.Visible;
            Create_Box_gluten.Visibility = ViewStates.Visible;
            Create_Box_lactose.Visibility = ViewStates.Visible;
            Create_AddAllergy.Visibility = ViewStates.Visible;
            Create_AddAllergyButton.Visibility = ViewStates.Visible;

            Created_AllergyProfile.AllergyList.Remove(Edited_AllergyItem);
            create_adapter.NotifyDataSetChanged();
        }

        // добавление ингредиента в список аллергий профиля
        private void Create_AddAllergyButton_Click(object sender, EventArgs e)
        {
            Create_Warning.Visibility = ViewStates.Gone;
            if (!String.IsNullOrWhiteSpace(Create_AddAllergy.Text))
            {
                if (Created_AllergyProfile.SeeIfAlreadyContains_Allergy(Create_AddAllergy.Text))
                {
                    string g = "0";
                    string l = "0";
                    if (Create_Box_gluten.Checked)
                    {
                        g = "1";
                    }

                    if (Create_Box_lactose.Checked)
                    {
                        l = "1";
                    }

                    Create_Box_lactose.Checked = false;
                    Create_Box_gluten.Checked = false;
                    Created_AllergyProfile.AllergyList.Add(new List<string> { Create_AddAllergy.Text, g, l });
                    Create_AddAllergy.Text = "";
                    create_adapter.NotifyDataSetChanged();
                }
                else
                {
                    Create_Warning.Visibility = ViewStates.Visible;
                    Create_Warning.Text = "Вы уже добавили элемент с таким названием.";
                }
            }
            else 
            {
                Create_Warning.Visibility = ViewStates.Visible;
                Create_Warning.Text = "Введите данные.";
            }
        }

        // ввод имени нового профиля
        private void Create_NameButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Create_NameEditable.Text))
            {
                Create_NameEditable.Visibility = ViewStates.Gone;
                Create_Name.Text= Create_NameEditable.Text;
                List<List<String>> BlankAllergyProfileList = new List<List<String>>();
                Created_AllergyProfile = new AllergyProfile(Create_NameEditable.Text, BlankAllergyProfileList,User.Current.Id);

                if (First_G.Checked)
                {
                    Created_AllergyProfile.AllergyList.Add(new List<string>() { "глютен", "1", "0" });
                    Created_AllergyProfile.G = 1;
                }
                if (First_L.Checked)
                {
                    Created_AllergyProfile.AllergyList.Add(new List<string>() { "лактоза", "0", "1" });
                    Created_AllergyProfile.L = 1;
                }
                
                First_G.Visibility = ViewStates.Gone;
                First_L.Visibility = ViewStates.Gone;
                First_Instruction.Text = "Нажмите на аллерген, чтобы удалить.";

                Create_Name.Visibility = ViewStates.Visible;
                CreateAllergies_ListView.Visibility = ViewStates.Visible;
                Create_SaveButton.Visibility = ViewStates.Visible;
                Create_AddAllergy.Visibility = ViewStates.Visible;
                Create_AddAllergyButton.Visibility = ViewStates.Visible;
                Create_DeleteButton.Visibility = ViewStates.Visible;
                Create_Box_gluten.Visibility = ViewStates.Visible;
                Create_Box_lactose.Visibility = ViewStates.Visible;
                Instruction.Visibility = ViewStates.Visible;
                Create_NameButton.Visibility = ViewStates.Gone;

                create_adapter = new AdapterPlain(this, Created_AllergyProfile.AllergyList);
                CreateAllergies_ListView.Adapter = create_adapter;

            }
        }

        // сохранение созданного профиля
        private async void Create_SaveButton_Click(object sender, EventArgs e)
        {
            Create_Warning.Visibility = ViewStates.Gone;

            if (!String.IsNullOrEmpty(Create_NameEditable.Text) && Created_AllergyProfile.AllergyList.Count != 0)
            {
                Create_Warning.Visibility = ViewStates.Visible;
                Create_Warning.Text = "пожалуйста, подождите";
                Created_AllergyProfile.Name = Create_NameEditable.Text;


                if (await SQLF.UploadData_NewProfile(Created_AllergyProfile))
                {
                    User.Current.AddAlllergyProfile(Created_AllergyProfile);
                    popupDialog_create_profile.Dismiss();
                    popupDialog_create_profile.Hide();
                }
                else
                {
                    Create_Warning.Text = SQLF.SQLF_answer_string;
                }

                profile_activity_adapter.NotifyDataSetChanged();
            }
            else 
            {
                Create_Warning.Visibility = ViewStates.Visible;
                Create_Warning.Text = "Введите данные";
            }
        }



        //EDIT PROFILE ACTION
        // редактирование профиля

        // выбор опции редактирование профиля
        private void ProfileActivityListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Edited_AllergyProfile = User.Current.AllergyProfileList[e.Position];
            popupDialog_edit_profile = new Dialog(this);
            popupDialog_edit_profile.SetContentView(Resource.Layout.profile_popup);
            popupDialog_edit_profile.Window.SetSoftInputMode(SoftInput.AdjustResize);
            popupDialog_edit_profile.Show();

            Edit_Name = popupDialog_edit_profile.FindViewById<TextView>(Resource.Id.profile_popup_name_text);

            EditAllergies_ListView = popupDialog_edit_profile.FindViewById<ListView>(Resource.Id.profile_popup_listview);
            Edit_SaveButton = popupDialog_edit_profile.FindViewById<Button>(Resource.Id.profile_popup_save_button);
            Edit_Warning = popupDialog_edit_profile.FindViewById<TextView>(Resource.Id.profile_popup_warning);
            Edit_YesButton = popupDialog_edit_profile.FindViewById<Button>(Resource.Id.profile_popup_yes_button);
            Edit_NoButton = popupDialog_edit_profile.FindViewById<Button>(Resource.Id.profile_popup_no_button);
            Edit_AddAllergy = popupDialog_edit_profile.FindViewById<EditText>(Resource.Id.profile_popup_add_text);
            Edit_AddAllergyButton = popupDialog_edit_profile.FindViewById<Button>(Resource.Id.profile_popup_add_allergy_button);
            Edit_DeleteButton = popupDialog_edit_profile.FindViewById<Button>(Resource.Id.profile_popup_delete_button);
            Edit_Box_gluten = popupDialog_edit_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_gluten);
            Edit_Box_lactose = popupDialog_edit_profile.FindViewById<CheckBox>(Resource.Id.profile_checkBox_lactose);
            Instruction = popupDialog_edit_profile.FindViewById<TextView>(Resource.Id.profile_checkbox_instruction);

            First_Instruction= popupDialog_edit_profile.FindViewById<TextView>(Resource.Id.profile_checkbox_instruction_name);
            First_Instruction.Text = "Нажмите на аллерген, чтобы удалить.";
            Edit_Name.Text = Edited_AllergyProfile.Name;
            edit_adapter = new AdapterPlain(this, Edited_AllergyProfile.AllergyList);
            EditAllergies_ListView.Adapter = edit_adapter;


            Edit_AddAllergyButton.Click += Edit_AddAllergyButton_Click;
            Edit_SaveButton.Click += Edit_SaveButton_Click;
            Edit_DeleteButton.Click += Edit_DeleteButton_Click;
            EditAllergies_ListView.ItemLongClick += EditAllergies_ListView_ItemLongClick;
        }

        // удаление ингредиента из списка аллергий профиля
        private void EditAllergies_ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Edited_AllergyItem = Edited_AllergyProfile.AllergyList[e.Position];
            Edit_Warning.Text = "Вы уверены, что хотите удалить данный аллерген?";

            Edit_Warning.Visibility = ViewStates.Visible;
            Edit_YesButton.Visibility = ViewStates.Visible;
            Edit_NoButton.Visibility = ViewStates.Visible;
            Edit_AddAllergyButton.Visibility = ViewStates.Visible;

            Edit_AddAllergy.Visibility = ViewStates.Gone;
            Edit_AddAllergyButton.Visibility = ViewStates.Gone;
            Instruction.Visibility = ViewStates.Gone;
            Edit_Box_gluten.Visibility=ViewStates.Gone;
            Edit_Box_lactose.Visibility=ViewStates.Gone;

            Edit_YesButton.Click += Edit_YesButton_Click;
            Edit_NoButton.Click += Edit_NoButton_Click;
        }

        // удаление ингредиента из списка аллергий профиля - выбор варианта нет
        private void Edit_NoButton_Click(object sender, EventArgs e)
        {
            Edit_Warning.Visibility = ViewStates.Gone;
            Edit_YesButton.Visibility = ViewStates.Gone;
            Edit_NoButton.Visibility = ViewStates.Gone;


            Edit_Box_gluten.Visibility = ViewStates.Visible;
            Edit_Box_lactose.Visibility = ViewStates.Visible;
            Instruction.Visibility = ViewStates.Visible;
            Edit_AddAllergy.Visibility = ViewStates.Visible;
            Edit_AddAllergyButton.Visibility = ViewStates.Visible;
        }

        // удаление ингредиента из списка аллергий профиля - выбор варианта да
        private async void Edit_YesButton_Click(object sender, EventArgs e)
        {
            Edit_Warning.Visibility = ViewStates.Gone;
            Edit_YesButton.Visibility = ViewStates.Gone;
            Edit_NoButton.Visibility = ViewStates.Gone;


            Edit_Box_gluten.Visibility = ViewStates.Visible;
            Edit_Box_lactose.Visibility = ViewStates.Visible;
            Instruction.Visibility = ViewStates.Visible;
            Edit_AddAllergy.Visibility = ViewStates.Visible;
            Edit_AddAllergyButton.Visibility = ViewStates.Visible;

            Edited_AllergyProfile.AllergyList.Remove(Edited_AllergyItem);
            AllergyProfile delete_profile_part=new AllergyProfile(Edited_AllergyProfile.Name, new List<List<string>>() { Edited_AllergyItem },Edited_AllergyProfile.User_Id);

            if (! await SQLF.DeleteData_ProfilePart(delete_profile_part)) 
            {
                Edit_Warning.Text = SQLF.SQLF_answer_string;
            }
            edit_adapter.NotifyDataSetChanged();
        }

        // удаление выбранного профиля
        private async void Edit_DeleteButton_Click(object sender, EventArgs e)
        {
            Edit_Warning.Visibility = ViewStates.Visible;
            Edit_Warning.Text= "пожалуйста, подождите";
            if (!await SQLF.DeleteData_Profile(Edited_AllergyProfile))
            {
                Edit_Warning.Text=SQLF.SQLF_answer_string;
            }
            else 
            {
                User.Current.AllergyProfileList.Remove(Edited_AllergyProfile);
                popupDialog_edit_profile.Dismiss();
                popupDialog_edit_profile.Hide();
            }
           
            profile_activity_adapter.NotifyDataSetChanged();
        }

        // сохранение изменений в выбранном профиле
        private async void Edit_SaveButton_Click(object sender, EventArgs e)
        {
            Edit_Warning.Visibility = ViewStates.Visible;
            if (Edit_Name.Text != "" && Edited_AllergyProfile.AllergyList.Count != 0)
            {
                Edited_AllergyProfile.SeeIfContains_Profile();
                Edit_Warning.Text = "пожалуйста, подождите";
                if (await SQLF.UpdateData_Profile(Edited_AllergyProfile))
                {
                    popupDialog_edit_profile.Dismiss();
                    popupDialog_edit_profile.Hide();
                    profile_activity_adapter.NotifyDataSetChanged();
                }
                else
                {
                    Edit_Warning.Text = SQLF.SQLF_answer_string;
                }
            }
            else 
            {
                Edit_Warning.Visibility = ViewStates.Visible;
                Edit_Warning.Text = "Введите данные.";
            }
        }

        // добавить ингредиент в список аллергий профиля
        private void Edit_AddAllergyButton_Click(object sender, EventArgs e)
        {
            Edit_Warning.Visibility = ViewStates.Gone;
            if (!String.IsNullOrWhiteSpace(Edit_AddAllergy.Text))
            {
                if (Edited_AllergyProfile.SeeIfAlreadyContains_Allergy(Edit_AddAllergy.Text))
                {
                    string g = "0";
                    string l = "0";
                    if (Edit_Box_gluten.Checked)
                    {
                        g = "1";
                    }

                    if (Edit_Box_lactose.Checked)
                    {
                        l = "1";
                    }

                    Edit_Box_lactose.Checked = false;
                    Edit_Box_gluten.Checked = false;
                    Edited_AllergyProfile.AllergyList.Add(new List<string> { Edit_AddAllergy.Text, g, l });
                    Edit_AddAllergy.Text = "";
                    edit_adapter.NotifyDataSetChanged();
                }
                else 
                {
                    Edit_Warning.Visibility = ViewStates.Visible;
                    Edit_Warning.Text= "Вы уже добавили элемент с таким названием.";
                }
            }
            else
            {
                Edit_Warning.Visibility = ViewStates.Visible;
                Edit_Warning.Text = "Введите данные.";
            }
        }

    }
}