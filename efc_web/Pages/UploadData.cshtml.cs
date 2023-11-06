using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace web_thingy.Pages
{


    public class UploadDataModel : PageModel
    {
        int type_response;
        string text, query;
        SqlConnection con;

        // Onpost:
        // получает RequestClass, где type_request - тип операции, которую нужно совершить, content_request - данные, необходимые для совершения указанной операции
        // возвращает ResponseClass, где type_response - успешность выполнения (0- не успешно, 1- успешно и 2- вариативный случай), content_response- возвращаемые данные или сообщение об ошибке
        public async Task<IActionResult> OnPost()
        {
            string raw_body = await Helpers.ExtractBodyAsync(Request.Body);
            var upld = JsonConvert.DeserializeObject<RequestClass>(raw_body);

            type_response = 0;
            text = "Что-то пошло не так.";
            try
            {
                if (upld != null)
                {

                    con = new SqlConnection(Helpers.connectionString);
                    con.Open();
                    if (upld.type_request == "upload_user")
                    {
                        // firebase_uid, email, password
                        var user_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (user_list != null) 
                        {
                            var firebase_uid = user_list[0];
                            var email = user_list[1];
                            var password = user_list[2];
                            Upload_User(firebase_uid,email,password);
                        }

                    }

                    else if (upld.type_request == "upload_item")
                    {
                        var item = JsonConvert.DeserializeObject<Item>(upld.content_request);
                        if (item != null)
                        {

                            Upload_Item(item);

                        }
                    }

                    else if (upld.type_request == "upload_profile")
                    {
                        var profile = JsonConvert.DeserializeObject<AllergyProfile>(upld.content_request);
                        if (profile != null)
                        {
                            Upload_Profile(profile);
                        }

                    }

                    else if (upld.type_request == "delete_profile")
                    {
                        var profile = JsonConvert.DeserializeObject<AllergyProfile>(upld.content_request);
                        if (profile != null)
                        {
                            Delete_Profile(profile);
                        }
                    }

                    else if (upld.type_request == "delete_profile_part")
                    {
                        var profile = JsonConvert.DeserializeObject<AllergyProfile>(upld.content_request);

                        if (profile != null)
                        {
                            Delete_Profile_Part(profile);
                        }
                    }

                    else if (upld.type_request == "update_profile")
                    {
                        var profile = JsonConvert.DeserializeObject<AllergyProfile>(upld.content_request);
                        if (profile != null)
                        {
                            if (Upload_ProfileContent(profile.AllergyList, profile.User_Id.ToString(), profile.Name)) { type_response = 1; text = "Успешно."; };
                        }

                    }

                    else if (upld.type_request == "upload_favourite")
                    {
                        var upld_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (upld_list != null)
                        {
                            var user_id = upld_list[0];
                            var item_id = upld_list[1];
                            Upload_Favourite(user_id, item_id);
                        }
                    }

                    else if (upld.type_request == "upload_history")
                    {
                        var p = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (p != null)
                        {
                            var user_id = p[0];
                            var item_id = p[1];
                            Upload_History(user_id, item_id);
                        }
                    }

                    else if (upld.type_request == "update_password")
                    {
                        // 1- user id, 2- password

                        var password_list = JsonConvert.DeserializeObject<List<string>>(upld.content_request);

                        if (password_list != null)
                        {
                            var user_id = password_list[0];
                            var password = password_list[1];
                            Update_Password(user_id, password);
                        }
                    }

                    else if (upld.type_request == "delete_fromuserlist")
                    {
                        var p = JsonConvert.DeserializeObject<List<string>>(upld.content_request);
                        if (p != null)
                        {
                            var user_id = p[0];
                            var item_id = p[1];
                            var tablename = p[2];
                            Delete_From_User_List(user_id,item_id,tablename);
                        }
                    }


                    con.Close();
                }
            }
            catch 
            {
                type_response = 0;
                text = "Ошибка работы сервера.";
            }
            ResponseClass response = new ResponseClass(type_response, text);

            return Content(JsonConvert.SerializeObject(response));
        }

        //USER

        // создание нового аккаунта, параметры - уникальный айди firebase, email и пароль
        public void Upload_User(string firebase_uid, string email, string password) 
        {
            try
            {
                var id = Helpers.CallForLastId_InsertVersion("UserTable", con);
                query = "INSERT INTO UserTable (id,firebase_uid,email,password) VALUES (" + id + ",'" + firebase_uid + "','" + email + "','" + password + "')";
                var cmd = new SqlCommand(query, con);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    query = "select id from UserTable where firebase_uid = '" + firebase_uid + "'";

                    cmd = new SqlCommand(query, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (!dr.IsDBNull("id"))
                        {
                            text = dr["id"].ToString();
                            type_response = 1;
                        }

                    }
                    dr.Close();
                }
            }
            catch { return; }
        }

        //обновить пароль, параметра - айди аккаунта и новый пароль
        public void Update_Password(string user_id, string password )
        {
            try 
            {
                query = "UPDATE UserTable SET password='" + password + "' WHERE id=" + user_id;
                var cmd = new SqlCommand(query, con);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    type_response = 1;
                    text = "Успешно.";
                }
            }
            catch { return; }
        }

        //удалить продукт из списка любимых/списка-истории, параметры - айди пользователя, айди товара, название таблицы
        public void Delete_From_User_List(string user_id, string item_id, string tablename)
        {
            try 
            {
                query = "IF EXISTS (SELECT * FROM " + tablename + " WHERE user_id=" + user_id + " AND item_id=" + item_id + ") BEGIN DELETE FROM " + tablename + " WHERE user_id=" + user_id + " AND item_id=" + item_id + " END";

                var cmd = new SqlCommand(query, con);

                if (cmd.ExecuteNonQuery() != -1)
                {
                    type_response = 1;
                    text = "Успешно.";
                }
            }
            catch { return; }
        }

        //ITEM

        //создание нового продукта, параметр - создаваемый продукт
        public void Upload_Item(Item item)
        {
            try
            {
                var id = Helpers.CallForLastId_InsertVersion("ItemTable", con);
                query = "IF NOT EXISTS (SELECT * FROM ItemTable WHERE barcode=" + item.BarCode.ToString() + ") BEGIN INSERT INTO ItemTable (id,name,contains_g,contains_l,barcode) VALUES(" + id + ",'" + item.Name + "'," + item.Contains_G.ToString() + "," + item.Contains_L.ToString() + "," + item.BarCode.ToString() + ") END";
                var cmd = new SqlCommand(query, con);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    query = "select id from ItemTable where barcode = " + item.BarCode;

                    cmd = new SqlCommand(query, con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (!dr.IsDBNull("id"))
                        {
                            text = dr["id"].ToString();
                            dr.Close();
                            if (Upload_ItemContent(item.Ingredients, text)) { type_response = 1; }
                        }
                    }
                    else
                    {
                        dr.Close();
                    }

                }
                else
                {
                    text = "Товар с таким кодом уже существует";
                    type_response = 2;
                }
            }
            catch { return; }
        }

        //добавление нового товара в любимые, параметры - айди пользователя и айди товара
        public void Upload_Favourite(string user_id, string item_id )
        {
            try 
            {
                string lastid = "-1";
                string query_lastid = "SELECT TOP 1 * FROM FavouritesTable WHERE user_id=" + user_id + " ORDER BY id DESC";
                var cmd = new SqlCommand(query_lastid, con);
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
                var insert_id = Helpers.CallForLastId_InsertVersion("FavouritesTable", con);
                query = "IF NOT EXISTS (SELECT * FROM FavouritesTable WHERE user_id=" + user_id + " AND item_id=" + item_id + " AND id="+lastid+") BEGIN INSERT INTO FavouritesTable(id,user_id,item_id) VALUES(" + insert_id + "," + user_id + ", " + item_id + ") END";
                cmd = new SqlCommand(query, con);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    type_response = 1;
                    text = "Успешно.";
                }
            }
            catch { return; }
        }

        //добавление нового товара в историю, параметры - айди пользователя и айди товара
        public void Upload_History(string user_id, string item_id)
        {
            try 
            {
                string lastid = "-1";
                string query_lastid = "SELECT TOP 1 * FROM HistoryTable WHERE user_id=" + user_id + " ORDER BY id DESC";
                var cmd = new SqlCommand(query_lastid, con);
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
                var insert_id = Helpers.CallForLastId_InsertVersion("HistoryTable", con);
                query = "IF NOT EXISTS (SELECT * FROM HistoryTable WHERE user_id=" + user_id + " AND item_id=" + item_id + " AND id=" + lastid + ") BEGIN INSERT INTO HistoryTable(id,user_id,item_id) VALUES(" + insert_id + "," + user_id + ", " + item_id + ") END";
                cmd = new SqlCommand(query, con);
                if (cmd.ExecuteNonQuery() != -1)
                {
                    type_response = 1;
                    text = "Успешно.";
                }
            }
            catch { return; }
        }

        // PROFILE

        // создание нового пользователя, параметр - создаваемый пользователь
        public void Upload_Profile(AllergyProfile profile)
        {
            try 
            {
                query = "SELECT * FROM AllergyProfileTable WHERE user_id=" + profile.User_Id.ToString() + " AND name='" + profile.Name + "'";
                var cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Close();
                    text = "Профиль с таким именем уже существует";
                    type_response = 2;
                }
                else
                {
                    dr.Close();
                    if (Upload_ProfileContent(profile.AllergyList, profile.User_Id.ToString(), profile.Name)) { type_response = 1; text = "Успешно."; };
                }
            }
            catch { return; }
        }

        // удаление пользователя, параметр-удаляемый пользователь
        public void Delete_Profile(AllergyProfile profile)
        {
            try 
            {
                query = "IF EXISTS (SELECT * FROM AllergyProfileTable WHERE name='" + profile.Name + "' AND user_id=" + profile.User_Id.ToString() + ") BEGIN DELETE FROM AllergyProfileTable WHERE name='" + profile.Name + "' AND user_id=" + profile.User_Id.ToString() + " END";

                var cmd = new SqlCommand(query, con);

                if (cmd.ExecuteNonQuery() != -1)
                {
                     type_response = 1;
                     text = "Успешно.";
                }
            }
            catch { return; }
        }

        // удаление аллергии из списка аллегрий пользователя, параметр - пользователь, в списке аллергий которого происходят изменения
        public void Delete_Profile_Part(AllergyProfile profile)
        {
            try 
            {
                var ing_id = Helpers.Get_IngredientId(profile.AllergyList[0], con);
                if (ing_id != "-1")
                {
                    query = "IF EXISTS (SELECT * FROM AllergyProfileTable WHERE name='" + profile.Name + "' AND user_ID=" + profile.User_Id + " AND ingredient_id=" + ing_id + ") BEGIN DELETE FROM AllergyProfileTable WHERE name='" + profile.Name + "' AND user_ID=" + profile.User_Id + " AND ingredient_id=" + ing_id + " END";
                    var cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    type_response = 1;
                    text = "Успешно.";
                }
            }
            catch { return; }
        }

        //загрузить данные пользователя, параметры - данные пользователя
        public bool Upload_ProfileContent(List<List<string>> list, string user_id,string name) 
        {
            bool c = true;
            try
            {
                foreach (var el in list)
                {
                    string el_id = Helpers.Get_IngredientId(el, con);
                    if (el_id != "-1")
                    {
                        string new_id = Helpers.CallForLastId_InsertVersion("AllergyProfileTable", con);
                        query = "IF NOT EXISTS (SELECT * FROM AllergyProfileTable WHERE ingredient_id =" + el_id + " AND name='" + name + "' AND user_id=" + user_id + ") BEGIN insert into AllergyProfileTable(id,name,ingredient_id,user_id,gluten,lactose) values(" + new_id + ",'" + name + "'," + el_id + "," + user_id + "," + el[1].ToString() + "," + el[2].ToString() + ") END";
                        var cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch 
            {
                c=false;
            }
            return c;

        }

        //загрузить данные продукта, параметры - данные продукта
        public bool Upload_ItemContent(List<List<string>> list, string item_id)
        {
            bool c = true;
            try
            {
                foreach (var el in list)
                {
                    string el_id = Helpers.Get_IngredientId(el, con);
                    if (el_id != "-1")
                    {
                        string new_id = Helpers.CallForLastId_InsertVersion("ItemContent",con);
                        query = "IF NOT EXISTS (SELECT * FROM ItemContent WHERE ingredient_id =" + el_id + " and item_id="+item_id+") BEGIN insert into ItemContent(id,item_id,ingredient_id) values(" + new_id + "," + item_id + "," + el_id + ") END";
                        var cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch 
            {
                c= false;
            }
            return c;

            
        }
    }
}