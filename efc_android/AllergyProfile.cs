using System.Collections.Generic;

namespace efc_final
{
    // класс, содержит информацию о персонаже (имя) и его аллергиях 
    public class AllergyProfile
    {
        public AllergyProfile() { }
        public string Name { get; set; }
        public int User_Id { get; set; }
        public List<List<string>> AllergyList { get; set; }

        public int G;

        public int L;

        public List<string> Allergies_Default= new List<string> { "Глютен","Лактоза"};

        // проверить, содержится ли аллерген в списке , параметры - аллерген
        public bool SeeIfAlreadyContains_Allergy(string allergy) 
        {
            bool c = true;
            foreach (var el in this.AllergyList) 
            {
                if (el[0] == allergy) { c = false; }
            }
            return c;
        }

        // получить список названий ингредиентов, параметр - список ингредиентов с полной характеристикой
        static public List<string> Get_AllergyListString(List<List<string>> a) 
        {
            var g = new List<string>();
            foreach (var el in a) 
            {
                g.Add(el[0]);
            }
            return g;
        }

        // проверить, есть ли у пользователя аллергия на глютен/лактозу
        public void SeeIfContains_Profile()
        {
            this.G = 0;
            this.L = 0;
            foreach (var el in this.AllergyList)
            {
                if (el[0] == "глютен")
                {
                    this.G = 1;
                }
                if (el[0] == "лактоза")
                {
                    this.L = 1;
                }
            }
        }

        public AllergyProfile(string name, List<List<string>> list, int user_Id)
        {
            this.Name = name;
            this.AllergyList = list;
            User_Id = user_Id;
            G = 0;
            L = 0;
        }
    }
}