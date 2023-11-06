using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace efc_final
{
    // вспомогательный класс для адаптера string
    public class ViewHolderPlain : Java.Lang.Object
    {
        public TextView txtName { get; set; }

        public TextView txtAllergy { get; set; }

    }
    // класс адаптер, используется для демонстрации списков List<string> через Listview
    public class AdapterPlain: BaseAdapter
    {
        private Activity activity;
        private List<List<string>> wordlist;


        public AdapterPlain(Activity activity, List<List<string>> list)
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.adapter_plain_layout, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.adapter_plain_textview);
            var txtAllergy= view.FindViewById<TextView>(Resource.Id.adapter_plain_textview_small);

            string txtAllergy_string="";
            if (wordlist[position][1] == "1") 
            {
                txtAllergy_string = "глютен";
            }

            if (wordlist[position][2] == "1")
            {
                if (txtAllergy_string.Count() == 0)
                {
                    txtAllergy_string = "лактоза";
                }
                else 
                {
                    txtAllergy_string = txtAllergy_string + ", лактоза";
                }

            }

            txtAllergy.Text = txtAllergy_string;
            txtName.Text = wordlist[position][0];

            return view;
        }
    }
}