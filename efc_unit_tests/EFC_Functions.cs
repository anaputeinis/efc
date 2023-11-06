using System;
using System.Collections.Generic;
using System.Linq;


namespace UnitTestProjectForEFC
{
    public class EFC_Functions
    {
        // проверить, есть ли в списке list пользователь с именем как у пользователя profile
        public static void Check_ProfileList(List<AllergyProfile> list, AllergyProfile profile)
        {
            bool c = true;
            foreach (var el in list)
            {
                if (el.Name == profile.Name)
                {
                    c = false;
                    bool cont = true;
                    foreach (var e in el.AllergyList)
                    {
                        if (e[0] == profile.AllergyList[0][0])
                        {
                            cont = false;
                        }
                    }
                    if (cont)
                    {
                        el.AllergyList.Add(profile.AllergyList[0]);
                    }
                    break;
                }
            }
            if (c)
            {
                list.Add(profile);
            }
            return;
        }


    }

    public class AllergyProfile
    {
        public AllergyProfile()
        {
            AllergyList = new List<List<string>>();
        }
        public string Name { get; set; }

        public int User_Id { get; set; }

        public List<List<string>> AllergyList { get; set; }

        public int G;

        public int L;

        public AllergyProfile(string name, List<List<string>> list, int user_Id)
        {
            this.Name = name;
            this.AllergyList = list;
            User_Id = user_Id;
            G = 0;
            L = 0;
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<List<string>> Ingredients { get; set; }
        public Int64 BarCode { get; set; }

        public int Contains_G;

        public int Contains_L;

        public Item() { }

        public Item(string name, List<List<string>> ingredients, Int64 bc)
        {
            Name = name;
            Ingredients = ingredients;
            BarCode = bc;
        }

        // проверить, содержит ли продукт лактозу или глютен
        public void SeeIfContains_Item()
        {
            this.Contains_G = 0;
            this.Contains_L = 0;
            foreach (var el in this.Ingredients)
            {
                if (el[1] == "1")
                {
                    this.Contains_G = 1;
                }
                if (el[2] == "1")
                {
                    this.Contains_L = 1;
                }
            }
        }

        // получить список из названий ингредиентов
        public static List<string> Get_IngredientsString(List<List<string>> l)
        {
            var s = new List<string>();

            foreach (var el in l)
            {
                s.Add(el[0]);
            }

            return s;
        }

        // приведение списка ингредиентов к стандартному виду, параметр - список в строковом виде
        public static List<string> Ingredients_Normalize(string start)
        {
            List<string> l = start.Split(',').ToList();
            for (int i = 0; i < l.Count(); i++)
            {
                string s = l[i];
                string new_s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
                l[i] = new_s.Trim();
            }
            l.RemoveAll(x => x == "");
            l = l.Distinct().ToList();
            return l;

        }
    }
}
