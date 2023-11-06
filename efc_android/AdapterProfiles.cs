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
    // вспомогательный класс для адаптера AllergyProfile
    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtName { get; set; }
        public TextView txtAllergyList { get; set; }
    }
    // класс адаптер для объектов AllergyProfile, используется для демонстрации списков List<AllergyProfile> через Listview
    public class AdapterProfiles : BaseAdapter
    {
        private Activity activity;
        private List<AllergyProfile> wordlist;


        public AdapterProfiles(Activity activity, List<AllergyProfile> list)
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
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.adapter_profile_layout, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.adapter_name);
            var txtAllergyList = view.FindViewById<TextView>(Resource.Id.adapter_allergylist);



            txtName.Text = wordlist[position].Name;
            txtAllergyList.Text = String.Join(", ", AllergyProfile.Get_AllergyListString(wordlist[position].AllergyList));

            return view;
        }
    }
}