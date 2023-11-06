using Microsoft.Data.SqlClient;
using System.Data;

namespace web_thingy
{
    // класс запроса, подробнее - в OnPost
    public class RequestClass
    {
        public string type_request { get; set; }

        public string content_request { get; set; }

        public RequestClass() { }

        public RequestClass(string Type_upload, string Data_upload)
        {
            this.type_request = Type_upload;
            this.content_request = Data_upload;
        }
    }

    // класс ответа, подробнее - в OnPost
    public class ResponseClass
    {
        public int type_response;

        public string content_response;

        public ResponseClass() { }

        public ResponseClass(int t,string s) 
        {
            type_response = t;
            content_response = s;
        }

    }

    // продукт
    public class Item
    {
        public Item() 
        {
            Ingredients = new List<List<string>>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<List<string>> Ingredients { get; set; }
        public Int64 BarCode { get; set; }

        public int Contains_G;

        public int Contains_L;


    }

    // пользователь
    public class AllergyProfile
    {
        public AllergyProfile() 
        {
            AllergyList=new List<List<string>>();
        }
        public string Name { get; set; }

        public int User_Id {get;set;}

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

        public void SeeIfContains_Profile()
        {
            this.G = 0;
            this.L = 0;
            foreach (var el in this.AllergyList)
            {
                if (el[1] == "1")
                {
                    this.G = 1;
                }
                if (el[2] == "1")
                {
                    this.L = 1;
                }
            }
        }
    }

    // аккаунт
    public class User
    {

        public int Id { get; set; }

        public string FirebaseUID { get; set; }

        public List<AllergyProfile> AllergyProfileList { get; set; }

        public List<Item> FavouriteItems { get; set; }

        public List<Item> History { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public User() 
        {
            AllergyProfileList = new List<AllergyProfile>();
            History = new List<Item>();
            FavouriteItems = new List<Item>();
        }

        public User(string uid, string email, string password)
        {
            FirebaseUID = uid;
            AllergyProfileList = new List<AllergyProfile>();
            FavouriteItems = new List<Item>();
            History = new List<Item>();
            Email = email;
            Password = password;
        }
    }

    // вспомогательные функции, con во всех параметрах - используемое SqlConnection
    public class Helpers
    {
        // строка подключения
        public const string connectionString = "Data Source=localhost;Initial Catalog=efc_final;User Id=efc; Password=71421efc;TrustServerCertificate=True";

        //получение запроса для OnPost
        public static async Task<string> ExtractBodyAsync(Stream body)
        {
            string data;
            using (StreamReader reader = new StreamReader(body))
            {
                data = await reader.ReadToEndAsync();
            }
            return data;
        }

        //получение последнего айди из таблицы, параметры- название таблицы
        public static string CallForLastId(string tablename, SqlConnection con)
        {
            string lastid = "-1";
            string query_lastid = "SELECT TOP 1 * FROM " + tablename + " ORDER BY id DESC";
            SqlCommand cmd = new SqlCommand(query_lastid, con);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read() && dr.HasRows)
            {
                lastid = dr["id"].ToString();

            }
            else
            {
                lastid = "0";
            }
            dr.Close();
            return lastid;
        }

        // получение айди для добавления новой строки в таблицу (вызов последнего и плюс 1), параметры- название таблицы
        public static string CallForLastId_InsertVersion(string tablename, SqlConnection con) 
        {
            string id= (Int32.Parse(Helpers.CallForLastId(tablename, con)) + 1).ToString();
            return id;

        }

        // получение списка Item по списку айди, параметры - список айди 
        public static List<Item> GetItemList_ByIdList(List<string> list, SqlConnection con)
        {
            var return_list = new List<Item>();
            foreach (var item_id in list)
            {
                var item = GetItem_ById(item_id, con);
                if (!return_list.Contains(item))
                {
                    return_list.Add(item);
                }
            }
            return return_list;
        }

        // получение Item по его айди, параметр - айди
        public static Item GetItem_ById(string item_id, SqlConnection con)
        {
            var item = new Item();
            item.Id=Int32.Parse(item_id);
            var query = "select * from ItemContent  where item_id =" + item_id;
            var cmd = new SqlCommand(query, con);
            var adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            var data = new DataTable();
            adapter.Fill(data);
            List<string> item_ingredient_ids = data.AsEnumerable().Select(x => x["ingredient_id"].ToString()).ToList().Distinct().ToList();
            item.Ingredients = GetIngredientList_ByIdList(item_ingredient_ids,con);
            query = "select * from ItemTable where id=" + item_id;
            cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            //dr.IsDBNull()
            if (dr.Read())
            {
                if (!dr.IsDBNull("name") && !dr.IsDBNull("barcode")  && !dr.IsDBNull("contains_l") && !dr.IsDBNull("contains_g"))
                {
                    item.Name = dr["name"].ToString();
                    item.BarCode = Int64.Parse(dr["barcode"].ToString());
                    item.Contains_G = Int32.Parse(dr["contains_g"].ToString());
                    item.Contains_L = Int32.Parse(dr["contains_l"].ToString());

                }
            }
            dr.Close();

            return item;
        }

        // получение списка названий ингредиентов по списку их айди, параметры - список айди
        public static  List<List<string>> GetIngredientList_ByIdList(List<string> list, SqlConnection con)
        {
            var return_list = new List<List<string>>();

            if (list != null)
            {
                foreach (var ing in list)
                {
                    var ing_list = GetIngredientName_ById(ing,con);
                    if (!return_list.Contains(ing_list))
                    {
                        return_list.Add(ing_list);
                    }
                }
            }

            return return_list;

        }

        // получение названия ингредиента по его айди, параметр- айди
        public static List<string> GetIngredientName_ById(string ing_id,SqlConnection con)
        {
            List<string> Return_Name = new List<string>();
            string query = "SELECT name, contains_g, contains_l FROM IngredientsTable WHERE id=" + ing_id;
            var cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                if (!dr.IsDBNull("name") && !dr.IsDBNull("contains_g") && !dr.IsDBNull("contains_l"))
                {
                    Return_Name.Add(dr["name"].ToString());
                    Return_Name.Add(dr["contains_g"].ToString());
                    Return_Name.Add(dr["contains_l"].ToString());
                }
                
            }
            dr.Close();
            return Return_Name;
        }

        // получение айди ингредеинта по его названию (причем если такого ингредиента не существует - он создается), параметр - название, содержание глютена, содержание лактозы
        public static string Get_IngredientId(List<string> n, SqlConnection con)
        {
            string return_id = "-1";
            string id = (Int32.Parse(Helpers.CallForLastId("IngredientsTable",con)) + 1).ToString();
            string query = "IF NOT EXISTS (SELECT * FROM IngredientsTable WHERE name='" + n[0] + "') BEGIN insert into IngredientsTable(id,name,contains_g,contains_l) values(" + id + ",'" + n[0] + "'," + n[1] + "," + n[2] + ") END";
            var cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            query = "SELECT id FROM IngredientsTable WHERE name='" + n[0] + "'";

            cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                if (!dr.IsDBNull("id"))
                {
                    return_id = dr["id"].ToString();
                }
                
            }
            dr.Close();
            return return_id;
        }
    }
}
