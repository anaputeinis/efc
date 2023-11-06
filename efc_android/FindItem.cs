using Android.App;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace efc_final
{
    [Activity(Label = "FindItem", NoHistory = false)]
    public class FindItem : Activity
    {
        private ListView FindItem_ListView;
        private Button SearchButton;
        private EditText SearchText;
        private TextView SearchWarning;
        public static List<Item> SearchItemList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.find_item_layout);
            FindItem_ListView = FindViewById<ListView>(Resource.Id.find_item_listview);
            SearchButton = FindViewById<Button>(Resource.Id.find_item_search_button);
            SearchText = FindViewById<EditText>(Resource.Id.find_item_search);
            SearchWarning = FindViewById<TextView>(Resource.Id.find_item_warning);

            Refresh_SearchListView();
            SearchButton.Click += SearchButton_Click;

            FindItem_ListView.ItemClick += FindItem_ListView_ItemClick;

        }

        // при нажатии на элемент списка переход на страницу просмотра продукта
        private void FindItem_ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ItemPage_View.Selected_Item = SearchItemList[e.Position];
            StartActivity(new Android.Content.Intent(this, typeof(ItemPage_View)));
        }

        // выполнение поискового запроса
        private async void SearchButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(SearchText.Text)) 
            {
                if(await SQLF.Get_Search(SearchText.Text))
                {
                    SearchItemList=JsonConvert.DeserializeObject<List<Item>>(SQLF.SQLF_answer_string);
                    Refresh_SearchListView();
                    if (SearchItemList.Count() == 0)
                    {
                        SearchWarning.Text = "Ничего не было найдено. Попробуйте составить более детальный запрос.";
                    }
                    else 
                    {
                        SearchWarning.Text = "Если вы не нашли то, что искали, попробуйте составить более детальный запрос.";
                    }
                }
                else
                {
                    SearchWarning.Text = SQLF.SQLF_answer_string;
                }
            }
        }

        //обновление списка найденных продуктов
        private void Refresh_SearchListView() 
        {
            if (SearchItemList == null) 
            {
                SearchItemList = new List<Item>();
            }
            var adapter_finditem = new AdapterItems(this, SearchItemList);
            FindItem_ListView.Adapter = adapter_finditem;
        }
    }
}