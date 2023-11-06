using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace efc_final
{
    // класс, представляет единицу продукта
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

        // проверить наличие глютена/лактозы в товаре
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

        // получить список названий ингредиентов
        public static List<string> Get_IngredientsString(List<List<string>> l) 
        {
            var s=new List<string>();

            foreach (var el in l) 
            {
                s.Add(el[0]);
            }

            return s;
        }

        // приведение списка ингредиентов к стандартному виду, параметр - список в строковом виде
        public static List<string> Ingredients_Normalize(string start)
        {
            List<string> l = start.Split(",").ToList();
            for (int i = 0; i < l.Count(); i++)
            {
                string s = l[i];
                string new_s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
                l[i] = new_s.Trim();
            }
            l.RemoveAll(x => x == "");
            l=l.Distinct().ToList();
            return l;

        }

        // процесс конвертирования bitmap в byte, параметры - конвертируемый bitmap
        public static byte[] BitmapToByte(Android.Graphics.Bitmap bi)
        {
            MemoryStream stream = new MemoryStream();
            bi.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
            var by = new Byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(by, 0, (int)stream.Length);

            return by;
        }
    }
}