using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using static Android.Views.View;
namespace efc_final
{
    [Activity(Label = "DashBoard", Theme = "@style/AppTheme")]
    public class DashBoard : AppCompatActivity, IOnClickListener
    {
        Button  Scan_Button, Find_Button, SeeProfile_Button, Add_Button;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.dash_board_layout);

            //View  
            Scan_Button = FindViewById<Button>(Resource.Id.dashboard_scan_button);
            Find_Button = FindViewById<Button>(Resource.Id.dashboard_find_product_button);
            Add_Button = FindViewById<Button>(Resource.Id.dashboard_add_product_button);
            SeeProfile_Button = FindViewById<Button>(Resource.Id.see_profile_button);

            Scan_Button.SetOnClickListener(this);
            Find_Button.SetOnClickListener(this);
            Add_Button.SetOnClickListener(this);
            SeeProfile_Button.SetOnClickListener(this);

        }

        // реакция на нажатие кнопки, каждая кнопка отвечает за переход на новую страницу
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_scan_button)
            {
                StartActivity(new Intent(this, typeof(CameraPage)));
            }
            else if (v.Id == Resource.Id.dashboard_find_product_button)
            {
                FindItem.SearchItemList = new List<Item>();
                StartActivity(new Intent(this, typeof(FindItem)));
            }
            else if (v.Id == Resource.Id.dashboard_add_product_button) 
            {
                StartActivity(new Intent(this, typeof(AddItemCameraPage)));
            }
            else if (v.Id == Resource.Id.see_profile_button)
            {
                StartActivity(new Intent(this, typeof(ProfileActivity)));
            }
        }
       

        

    }
}