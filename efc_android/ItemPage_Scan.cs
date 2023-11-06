using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace efc_final
{
    [Activity(Label = "ItemPage")]
    public class ItemPage_Scan : Activity
    {
        private Button Scan_SaveButton,Scan_ScanAgainButton;
        private ListView Scan_ListView;
        private TextView Scan_Ingredients,Scan_Greeting;
        private List<AllergyProfile> AllergyProfileList_NotAllowed;
        private AdapterProfiles Scan_adapter;
        private Int64 Barcode;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.item_page_layout);
            Scan_SaveButton = FindViewById<Button>(Resource.Id.item_save_button);
            Scan_ScanAgainButton = FindViewById<Button>(Resource.Id.item_scan_more_button);
            Scan_Ingredients = FindViewById<TextView>(Resource.Id.for_scan_page);
            Scan_ListView = FindViewById<ListView>(Resource.Id.item_listview);
            Scan_Greeting= FindViewById<TextView>(Resource.Id.item_name_text);

            
            Scan_ScanAgainButton.Visibility = ViewStates.Visible;
            Scan_Ingredients.Visibility = ViewStates.Visible;
            Scan_ListView.Visibility = ViewStates.Visible;
            Scan_Greeting.Visibility = ViewStates.Visible;
            
            Scan_Greeting.Text = "пожалуйста, подождите";
            if (AllergyProfileList_NotAllowed == null) 
            {
                AllergyProfileList_NotAllowed = new List<AllergyProfile>();
            }
            
            var check = Int64.TryParse(CameraPage.TextResultRedacted, out Barcode);
            if(check)
            {
                Scan_Ingredients.Text = string.Empty;
                
                if (await SQLF.Get_ScanByBarcode(new List<string>() { User.Current.Id.ToString(), Barcode.ToString() }))
                {
                    Scan_Greeting.Text = "пользователи, которым не подойдет данный продукт:";
                    AllergyProfileList_NotAllowed = JsonConvert.DeserializeObject<List<AllergyProfile>>(SQLF.SQLF_answer_string);
                    Scan_adapter = new AdapterProfiles(this, AllergyProfileList_NotAllowed);
                    Scan_ListView.Adapter = Scan_adapter;

                    Scan_SaveButton.Visibility = ViewStates.Visible;
                    Scan_SaveButton.Text = "Просмотреть товар";
                    Scan_SaveButton.Click += Scan_ViewItemClick;

                }
                else 
                {
                    Scan_Greeting.Text = String.Empty;
                    AllergyProfileList_NotAllowed.Add(new AllergyProfile(SQLF.SQLF_answer_string, new List<List<string>>(), User.Current.Id));
                    Scan_adapter = new AdapterProfiles(this, AllergyProfileList_NotAllowed);
                    Scan_ListView.Adapter = Scan_adapter;
                }
            }
            else
            {
                Scan_Ingredients.Text = CameraPage.TextResultRedacted;
                if (await SQLF.Get_ScanByList(new List<string>() { User.Current.Id.ToString(), JsonConvert.SerializeObject(Item.Ingredients_Normalize(CameraPage.TextResultRedacted)) }))
                {
                    Scan_Greeting.Text = "пользователи, которым не подойдет данный продукт:";
                    AllergyProfileList_NotAllowed = JsonConvert.DeserializeObject<List<AllergyProfile>>(SQLF.SQLF_answer_string);
                    Scan_adapter = new AdapterProfiles(this, AllergyProfileList_NotAllowed);
                    Scan_ListView.Adapter = Scan_adapter;
                    Scan_SaveButton.Visibility = ViewStates.Visible;
                    Scan_SaveButton.Click += Scan_SaveButton_Click;
                }
                else 
                {
                    Scan_Greeting.Text = String.Empty;
                    AllergyProfileList_NotAllowed = new List<AllergyProfile>() { new AllergyProfile(SQLF.SQLF_answer_string, new List<List<string>>(),User.Current.Id ) };
                    Scan_adapter = new AdapterProfiles(this, AllergyProfileList_NotAllowed);
                    Scan_ListView.Adapter = Scan_adapter;
                    Scan_adapter.NotifyDataSetChanged();
                    Scan_SaveButton.Visibility = ViewStates.Visible;
                    Scan_SaveButton.Click += Scan_SaveButton_Click;

                }
                
            }
            

            
            Scan_ScanAgainButton.Click += Scan_ScanAgainButton_Click;
        }

        // переход на страницу просмотра продукта
        private async void Scan_ViewItemClick(object sender, EventArgs e)
        {
            ItemPage_View.Selected_Item = null;
            if (await SQLF.Get_Item(Barcode.ToString()))
            {
                ItemPage_View.Selected_Item = JsonConvert.DeserializeObject<Item>(SQLF.SQLF_answer_string);
            }
            
            StartActivity(new Android.Content.Intent(this, typeof(ItemPage_View)));
            Finish();
        }

        // начать процесс сканирования заново
        private void Scan_ScanAgainButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(CameraPage)));
            Finish();
        }
        
        //сохранить товар
        private void Scan_SaveButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(ItemPage_Save)));
            Finish();
        }
    }
}