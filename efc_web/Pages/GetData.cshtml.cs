using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace web_thingy.Pages
{
    public class GetDataModel : PageModel
    {
        int type_response;
        string text, query;
        SqlDataAdapter adapter;
        DataTable data;
        SqlConnection con;
        
        // Onpost:
        // получает RequestClass, где type_request - тип операции, которую нужно совершить, content_request - данные, необходимые для совершения указанной операции
        // возвращает ResponseClass, где type_response - успешность выполнения (0- не успешно, 1- успешно и 2- вариативный случай), content_response- возвращаемые данные или сообщение об ошибке
        public async Task<IActionResult> OnPost()
        {
            string raw_body = await Helpers.ExtractBodyAsync(Request.Body);
            var upld = JsonConvert.DeserializeObject<RequestClass>(raw_body);

            type_response = 0;
            text = "-1";
            con = new SqlConnection(Helpers.connectionString);
            con.Open();
            try
            {
                if (upld != null)
                {

                    if (upld.type_request == "get_idbyname")
                    {

                        var strings = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (strings != null)
                        {
                            string nametable = strings[0];
                            string namevariable = strings[1];
                            string variablevalue = strings[2];

                            Get_IdByName(nametable, namevariable, variablevalue);
                        }
                    }

                    else if (upld.type_request == "get_user_id")
                    {
                        var user_id = upld.content_request;
                        Get_UserId(user_id);
                    }

                    else if (upld.type_request == "get_user_profiles")
                    {
                        var user_id = upld.content_request;
                        Get_UserProfiles(user_id);
                    }

                    else if (upld.type_request == "get_user_list")
                    {
                        // 1-user id,2-table name
                        var upld_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        var Return_List = new List<Item>();
                        if (upld_list != null)
                        {
                            var user_id = upld_list[0];
                            var table_name = upld_list[1];

                            Get_UserList(user_id, table_name);
                        }
                    }

                    else if (upld.type_request == "get_search")
                    {
                        var search = upld.content_request;
                        Get_Search(search);
                        
                    }

                    else if (upld.type_request == "get_by_barcode")
                    {
                        // 1-user_id, 2-barcode
                        var upld_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (upld_list != null)
                        {
                            var user_id = upld_list[0];
                            var barcode = upld_list[1];
                            Get_ByBarcode(user_id, barcode);
                        }

                    }
                    else if (upld.type_request == "get_by_inglist")
                    {
                        // 1-user_id, 2-ings name list
                        var upld_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (upld_list != null)
                        {
                            var user_id = upld_list[0];
                            var ing_names = JsonConvert.DeserializeObject<List<string>>(upld_list[1]);
                            Get_ByIngList(user_id, ing_names);
                        }
                    }
                    else if (upld.type_request == "get_item")
                    {
                        var upload =upld.content_request;
                        var item_barcode = new string(upload.Where(char.IsNumber).ToArray());
                        Get_Item(item_barcode);
                        
                    }
                }
            }
            catch
            {
                type_response = 0;
                text = "Ошибка работы сервера.";
            }
            con.Close();

            ResponseClass response = new ResponseClass(type_response, text);

            return Content(JsonConvert.SerializeObject(response));

        }

        // получить данные аккаунта по его айди, параметр - айди аккаунта
        private void Get_UserId(string user_id) 
        {
            try 
            {
                var Return_User = new User();
                string firebase_uid = "";
                string email = "";
                string password = "";
                query = "select firebase_uid, email,password from UserTable where id = " + user_id;
                var cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    if ( !dr.IsDBNull("firebase_uid") && !dr.IsDBNull("email") && !dr.IsDBNull("password"))
                    {
                        firebase_uid = dr["firebase_uid"].ToString();
                        email = dr["email"].ToString();
                        password = dr["password"].ToString();

                        Return_User = new User(firebase_uid, email, password);
                        Return_User.Id = Int32.Parse(user_id);
                    }
                }
                dr.Close();
                type_response = 1;
                text = JsonConvert.SerializeObject(Return_User);
            }
            catch 
            {
                return;
            }
        }

        // получить пользователей аккаунта по айди, параметр - айди аккаунта
        private void Get_UserProfiles(string user_id)
        {
            try 
            {
                var ReturnUser_Profiles = new List<AllergyProfile>();

                query = "select  AllergyProfileTable.name from  AllergyProfileTable where user_id =" + user_id;
                var cmd = new SqlCommand(query, con);
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                data = new DataTable();
                adapter.Fill(data);

                List<string> ReturnUser_Names = data.AsEnumerable().Select(x => x["name"].ToString()).ToList().Distinct().ToList();
                if (ReturnUser_Names != null)
                {
                    foreach (var el in ReturnUser_Names)
                    {
                        var Profile = new AllergyProfile();
                        Profile.Name = el;
                        Profile.User_Id = Int32.Parse(user_id);
                        query = "select  * from UserTable Left Join AllergyProfileTable on AllergyProfileTable.name ='" + el + "' where AllergyProfileTable.user_id=" + user_id;
                        cmd = new SqlCommand(query, con);
                        adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        data = new DataTable();
                        adapter.Fill(data);

                        List<string> ReturnUser_IngedientListIds = data.AsEnumerable().Select(x => x["ingredient_id"].ToString()).Distinct().ToList();
                        Profile.AllergyList = Helpers.GetIngredientList_ByIdList(ReturnUser_IngedientListIds, con);

                        query = "select  * from AllergyProfileTable where name ='" + el + "' and user_id=" + user_id+" and gluten=1";
                        cmd = new SqlCommand(query, con);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read() && dr.HasRows)
                        {
                            Profile.G = 1;
                        }
                        dr.Close();

                        query = "select  * from AllergyProfileTable where name ='" + el + "' and user_id=" + user_id + " and lactose=1";
                        cmd = new SqlCommand(query, con);
                        dr = cmd.ExecuteReader();
                        if (dr.Read() && dr.HasRows)
                        {
                            Profile.G = 1;
                        }
                        dr.Close();

                        ReturnUser_Profiles.Add(Profile);
                    }
                }
                type_response = 1;
                text = JsonConvert.SerializeObject(ReturnUser_Profiles);
            }
            catch 
            {
                return;
            }
        }

        // получить список любимых продуктов/список-историю, параметры - айди аккаунта, название таблицы для считывания данных
        private void Get_UserList(string user_id, string table_name)
        {
            try 
            {
                var ReturnUser_List = new List<Item>();

                query = "select  TOP 25 * from " + table_name + " where user_id =" + user_id + " ORDER BY id DESC";
                var cmd = new SqlCommand(query, con);
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                data = new DataTable();
                adapter.Fill(data);

                List<string> item_ids = data.AsEnumerable().Select(x => x["item_id"].ToString()).ToList().Distinct().ToList();
                var Return_List = Helpers.GetItemList_ByIdList(item_ids, con);
                type_response = 1;
                text = JsonConvert.SerializeObject(Return_List);
            }
            catch { return; }
        }

        // получить список товаров, нашедшихся по поисковому запросу, параметры - текст поискового запроса
        private void Get_Search(string search)
        {
            try
            {
                var Return_List = new List<Item>();
                query = "SELECT TOP 20 * FROM ItemTable WHERE name LIKE '%" + search + "%' ORDER BY CASE WHEN name LIKE '" + search + "%' THEN 1 WHEN name LIKE '%" + search + "' THEN 3 END";
                var cmd = new SqlCommand(query, con);
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                data = new DataTable();
                adapter.Fill(data);

                List<string> item_ids = data.AsEnumerable().Select(x => x["id"].ToString()).ToList().Distinct().ToList();
                if (item_ids != null && item_ids.Count > 0) { Return_List = Helpers.GetItemList_ByIdList(item_ids, con); }
                type_response = 1;
                text = JsonConvert.SerializeObject(Return_List);
            }
            catch { return; }
        }

        // получить список пользователей аккаунта, которым нельзя употреблять продукт с указанным составом, параметры - айди аккаунта, список ингредиентов продукта
        private void Get_ByIngList(string user_id, List<string> ing_names)
        {
            try 
            {
                var ing_ids = new List<string>();
                foreach (var name in ing_names)
                {
                    query = "SELECT id FROM IngredientsTable WHERE name='" + name + "'";

                    var cmd = new SqlCommand(query, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (!dr.IsDBNull("id"))
                        {
                            ing_ids.Add(dr["id"].ToString());
                        }

                    }
                    dr.Close();
                }
                var result = Get_SelectedAllergyProfiles(ing_ids, user_id);
                if (result.Count() == 0) { result.Add(new AllergyProfile("Этот товар не содержит выбранных аллергенов.", new List<List<string>>(), Int32.Parse(user_id))); }
                text = JsonConvert.SerializeObject(result);
                type_response = 1;
            }
            catch { return; }
        }

        // получить список пользователей аккаунта, которым нельзя употреблять продукт с указанным штрихкодом, параметры - айди аккаунта, штрихкод продукта
        private void Get_ByBarcode(string user_id, string barcode)
        {
            try 
            {
                query = "SELECT * FROM ItemTable WHERE barcode=" + barcode;
                var cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    if (!dr.IsDBNull("id"))
                    {
                        var item_id = dr["id"].ToString();
                        dr.Close();
                        var query = "select * from ItemContent  where item_id =" + item_id;
                        cmd = new SqlCommand(query, con);
                        var adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        var data = new DataTable();
                        adapter.Fill(data);
                        List<string> item_ingredient_ids = data.AsEnumerable().Select(x => x["ingredient_id"].ToString()).ToList().Distinct().ToList();
                        var result = Get_SelectedAllergyProfiles(item_ingredient_ids, user_id);
                        if (result.Count == 0) { result.Add(new AllergyProfile("Этот товар не содержит выбранных аллергенов.", new List<List<string>>(), Int32.Parse(user_id))); }
                        text = JsonConvert.SerializeObject(result);
                        type_response = 1;
                    }
                }
                else
                {
                    dr.Close();
                    type_response = 2;
                    text = "Товар с таким штрихкодом не зарегистрирован. Попробуйте ввести состав вручную или с помощью камеры.";
                }

            }
            catch { return; }
        }

        // получить продукт по айди, параметры - айди товара
        private void Get_Item(string item_barcode)
        {
            try {  
   
                query = "SELECT * FROM ItemTable WHERE barcode=" + item_barcode;
                var cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    if (!dr.IsDBNull("id"))
                    {
                        var item_id = dr["id"].ToString();
                        dr.Close();
                        var item = Helpers.GetItem_ById(item_id, con);
                        type_response = 1;
                        text = JsonConvert.SerializeObject(item);
                    }
                    else { dr.Close(); }
                }
                else { dr.Close(); }
            }
            catch { return; }
        }

        // получить айди некого объекта из таблицы nametable, у которого поле namevariable имеет значение variablevalue
        private void Get_IdByName(string nametable,string namevariable,string variablevalue)
        {
            try 
            {
                query = "select id from " + nametable + " where " + namevariable + " = '" + variablevalue + "'";

                var cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    text = dr["id"].ToString();
                    type_response = 1;
                }
                dr.Close();
            }
            catch 
            {
                return;
            }

        }

        // проверить, есть ли у пользователей аккаунта с айди user_id аллергия на ингредиенты с айди из item_ing_ids (возвращает список пользователей)
        private List<AllergyProfile> Get_SelectedAllergyProfiles(List<string> item_ing_ids, string user_id)
        {
            bool G = false;
            bool L= false;
            List<AllergyProfile> selected_allergy_profiles = new List<AllergyProfile>();
            foreach (var ing_id in item_ing_ids)
            {
                query = "SELECT * FROM AllergyProfileTable WHERE user_id=" + user_id + " AND ingredient_id=" + ing_id;
                var cmd = new SqlCommand(query, con);
                var adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                var data = new DataTable();
                adapter.Fill(data);
                List<string> names = data.AsEnumerable().Select(x => x["name"].ToString()).ToList().Distinct().ToList();
                foreach (var name in names)
                {
                    var profile = new AllergyProfile();
                    profile.Name = name;
                    var ing_name = Helpers.GetIngredientName_ById(ing_id, con);
                    if (ing_name[0] != "-1")
                    {
                        if (!profile.AllergyList.Contains(ing_name))
                        {
                            profile.AllergyList.Add(ing_name);
                        }
                    }
                    Check_ProfileList(selected_allergy_profiles,profile);
                }

                query = "SELECT * FROM IngredientsTable WHERE id=" + ing_id;
                cmd = new SqlCommand(query, con);
                var dr = cmd.ExecuteReader();
                if (dr.Read() && dr.HasRows)
                {
                    if (!dr.IsDBNull("contains_g"))
                    {
                        if (dr["contains_g"].ToString() == "1") { G = true; }
                    }
                    if (!dr.IsDBNull("contains_l"))
                    {
                        if (dr["contains_l"].ToString() == "1") { L = true; }
                    }
                }
                dr.Close();

                if (G == true) 
                {
                    query = "SELECT * FROM AllergyProfileTable WHERE gluten=1 AND user_id=" + user_id;
                    cmd = new SqlCommand(query, con);
                    adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    data = new DataTable();
                    adapter.Fill(data);
                    List<string> names_g = data.AsEnumerable().Select(x => x["name"].ToString()).ToList().Distinct().ToList();
                    foreach (var name in names_g) 
                    {
                        var profile = new AllergyProfile();
                        profile.Name = name;
                        var g_list = new List<string>() { "глютен", "1", "0" };
                        if (!profile.AllergyList.Contains(g_list))
                        {
                            profile.AllergyList.Add(g_list);
                        }
                        Check_ProfileList(selected_allergy_profiles, profile);
                    }

                }

                if (L == true)
                {
                    query = "SELECT * FROM AllergyProfileTable WHERE lactose=1 AND user_id=" + user_id;
                    cmd = new SqlCommand(query, con);
                    adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    data = new DataTable();
                    adapter.Fill(data);
                    List<string> names_l = data.AsEnumerable().Select(x => x["name"].ToString()).ToList().Distinct().ToList();
                    foreach (var name in names_l)
                    {
                        var profile = new AllergyProfile();
                        profile.Name = name;
                        var l_list = new List<string>() { "лактоза", "0", "1" };
                        if (!profile.AllergyList.Contains(l_list))
                        {
                            profile.AllergyList.Add(l_list);
                        }
                        Check_ProfileList(selected_allergy_profiles, profile);
                    }

                }

            }
            return selected_allergy_profiles;
        }

        // проверить, есть ли в списке list пользователь с именем как у пользователя profile
        public void Check_ProfileList(List<AllergyProfile> list, AllergyProfile profile)
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
}
