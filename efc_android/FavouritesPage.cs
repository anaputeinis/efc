using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace efc_final
{
    [Activity(Label = "FavouritesPage")]
    public class FavouritesPage : Activity
    {
        private ListView FavouritesListView;
        private AdapterItems list_adapter;
        public static List<Item> FavouritesList;
        private TextView FavouritesGreeting, FavouritesWarning;
        private Dialog item_list_dialog;
        private Item Selected_Item;
        private Button ItemList_YesButton, ItemList_NoButton;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.item_list_layout);
            if (FavouritesList == null) 
            {
                FavouritesList = new List<Item>();
            }
            FavouritesListView = FindViewById<ListView>(Resource.Id.item_list);
            FavouritesGreeting = FindViewById<TextView>(Resource.Id.item_list_greeting);
            FavouritesWarning = FindViewById<TextView>(Resource.Id.item_list_warning);
            FavouritesGreeting.Text = "Ваши любимые продукты:";


            if (await SQLF.Get_UserList(new List<string>() { User.Current.Id.ToString(), "FavouritesTable" }))
            {
                FavouritesList = JsonConvert.DeserializeObject<List<Item>>(SQLF.SQLF_answer_string);
                list_adapter = new AdapterItems(this, FavouritesList);
                FavouritesListView.Adapter = list_adapter;
            }
            else 
            {
                FavouritesWarning.Visibility = ViewStates.Visible;
                FavouritesWarning.Text = SQLF.SQLF_answer_string;
                
            }

            FavouritesListView.ItemLongClick += FavouritesListView_ItemLongClick;
            FavouritesListView.ItemClick += FavouritesListView_ItemClick;


        }

        // нажатие на элемент списка открывает страницу с просмотром товара
        private void FavouritesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ItemPage_View.Selected_Item= FavouritesList[e.Position];
            StartActivity(new Android.Content.Intent(this, typeof(ItemPage_View)));
        }

        // долгое нажатие на элемент списка вызывает диалоговое окно с возможностью удалить
        private void FavouritesListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Selected_Item= FavouritesList[e.Position];
            item_list_dialog = new Dialog(this);
            item_list_dialog.SetContentView(Resource.Layout.remove_popup_layout);
            item_list_dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            item_list_dialog.Show();

            ItemList_YesButton = item_list_dialog.FindViewById<Button>(Resource.Id.remove_yes_button);
            ItemList_NoButton = item_list_dialog.FindViewById<Button>(Resource.Id.remove_no_button);

            ItemList_YesButton.Click += ItemList_YesButton_Click;
            ItemList_NoButton.Click += ItemList_NoButton_Click;
        }

        // удаление продукта - выбор нет
        private void ItemList_NoButton_Click(object sender, EventArgs e)
        { 
            item_list_dialog.Dismiss();
            item_list_dialog.Hide();
        }
        
        // удаление продукта - выбор да
        private async void ItemList_YesButton_Click(object sender, EventArgs e)
        {
            if (await SQLF.Delete_FromUserList(new List<string>() { User.Current.Id.ToString(), Selected_Item.Id.ToString(), "FavouritesTable" }))
            {
                TextView ItemListTextView = item_list_dialog.FindViewById<TextView>(Resource.Id.remove_textview);
                ItemList_YesButton.Visibility = ViewStates.Gone;
                ItemList_NoButton.Visibility = ViewStates.Gone;
                ItemListTextView.Text ="Продукт успешно удален.";
                FavouritesList.Remove(Selected_Item);
                list_adapter.NotifyDataSetChanged();
            }
            else
            {
                TextView ItemListTextView = item_list_dialog.FindViewById<TextView>(Resource.Id.remove_textview);
                ItemList_YesButton.Visibility = ViewStates.Gone;
                ItemList_NoButton.Visibility = ViewStates.Gone;
                ItemListTextView.Text = SQLF.SQLF_answer_string;
            }
        }
    }
}