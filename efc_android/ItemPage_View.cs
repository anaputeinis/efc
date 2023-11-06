using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace efc_final
{
    [Activity(Label = "ItemPage_View", NoHistory = false)]
    public class ItemPage_View : Activity
    {
        public static Item Selected_Item;
        private TextView Name,Barcode, View_Warning;
        private Button FavouritesButton;
        private ListView IngredientsListView;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.item_page_layout);
            

            IngredientsListView = FindViewById<ListView>(Resource.Id.item_listview);
            Name = FindViewById<TextView>(Resource.Id.item_name_text);
            Barcode = FindViewById<TextView>(Resource.Id.item_barcode_text);
            View_Warning = FindViewById<TextView>(Resource.Id.item_page_warning);
            FavouritesButton = FindViewById<Button>(Resource.Id.item_heart_button);


            if (Selected_Item == null)
            {
                Selected_Item = new Item();
                Name.Visibility = ViewStates.Visible;
                Name.Text = "Произошла ошибка получения товара.";
            }
            else
            {
                IngredientsListView.Visibility = ViewStates.Visible;
                Name.Visibility = ViewStates.Visible;
                Barcode.Visibility = ViewStates.Visible;
                
                FavouritesButton.Visibility = ViewStates.Visible;
                var view_adapter = new AdapterPlain(this, Selected_Item.Ingredients); 
                IngredientsListView.Adapter= view_adapter;
                Name.Text = Selected_Item.Name;
                Barcode.Text = Selected_Item.BarCode.ToString();

                await SQLF.Upload_History(new List<string>() { User.Current.Id.ToString(), Selected_Item.Id.ToString() });
                FavouritesButton.Click += FavouritesButton_Click;
            }

        }

        // добавление продукта в избранное
        private async void FavouritesButton_Click(object sender, EventArgs e)
        {
            View_Warning.Visibility = ViewStates.Gone;
            if (await SQLF.Upload_Favourite(new List<string>(){User.Current.Id.ToString(), Selected_Item.Id.ToString()})) 
            {
                View_Warning.Visibility = ViewStates.Visible;
                View_Warning.Text = "Товар успешно добавлен.";
            }
            else 
            {
                View_Warning.Visibility = ViewStates.Visible;
                View_Warning.Text = SQLF.SQLF_answer_string;
            }
        }
    }
}