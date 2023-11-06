using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace efc_final
{
    // вспомогательный класс для адаптера Items
    public class ViewHolderItems : Java.Lang.Object
    {
        public TextView txtName { get; set; }
        public TextView txtIngredients { get; set; }
        public TextView txtBr { get; set; }
    }
    // класс адаптер для объектов Items, используется для демонстрации списков List<Item> через Listview
    public class AdapterItems : BaseAdapter
    {
        private Activity activity;
        private List<Item> wordlist;


        public AdapterItems(Activity activity, List<Item> list)
        {
            this.activity = activity;
            this.wordlist = list;
        }

        public override int Count
        {
            get
            {
                return wordlist.Count();
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.adapter_items_layout, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.adapter_items_name);
            var txtIngredients = view.FindViewById<TextView>(Resource.Id.adapter_items_ingredients);
            var txtBr = view.FindViewById<TextView>(Resource.Id.adapter_items_qr);

            txtName.Text = wordlist[position].Name;
            txtIngredients.Text = String.Join(", ", Item.Get_IngredientsString(wordlist[position].Ingredients));
            txtBr.Text = wordlist[position].BarCode.ToString();
            return view;
        }
    }
}