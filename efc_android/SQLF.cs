using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace efc_final
{
    // класс, используемый для отправки запросов к веб приложению
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

    // класс, используемый для получения ответов из веб приложения
    public class ResponseClass
    {
        public int type_response;

        public string content_response;

        public ResponseClass() { }

        public ResponseClass(int t, string s)
        {
            type_response = t;
            content_response = s;
        }

    }


    // взаимодействие с веб приложением, каждое упоминание процесса создания подразумевает создание на SQL сервере
    // все функции возвращают true или false в зависимости от успешности операции
    public class SQLF
    {
        public SQLF() { }

        // адреса доступа к веб приложению (используемый во время разработки и актуальный)
        //public static string ConnectionString = "http://192.168.7.101:5001/";
        public static string ConnectionString = "http://dataregister.ru:8084/";

        public static string SQLF_answer_string;

        // создание нового продукта, параметр - добавляемый продукт
        public async static System.Threading.Tasks.Task<bool> UploadData_NewItem(Item item)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);

                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("upload_item", JsonConvert.SerializeObject(item)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;


                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка cохранения товара";
                                c = false;
                            }
                            else if (response_obj.type_response == 2)
                            {
                                SQLF_answer_string = response_obj.content_response;
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка cохранения товара";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // создание нового аккаунта, параметр - firebaseuid (уникальный идетнификатор каждого аккаунта)
        public async static System.Threading.Tasks.Task<bool> UploadData_NewUser(List<string> s) 
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("upload_user", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка создания нового пользвателя";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка создания нового пользвателя";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // обновление даных аккаунта, параметр- список из айди аккаунта и пароля
        public async static System.Threading.Tasks.Task<bool> UpdateData_User(List<string> s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                // 1- user id, 2- password
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("update_password", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка обновления пароля";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка обновления пароля";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // удаление продукта из списка любимых/истории, параметр - список из айди аккаунта, айди продукта и названия таблицы (из которой будет произведено удаление)
        public async static System.Threading.Tasks.Task<bool> Delete_FromUserList(List<string> s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                // 1- user id, 2- itemid, 3- tablename
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("delete_fromuserlist", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка удаления.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка удаления.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // добавление продукта в историю, список из айди аккаунта и айди продукта
        public async static System.Threading.Tasks.Task<bool> Upload_History(List<string> s)
        {if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(200);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("upload_history", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получить продукт по айди, параметр - айди продукта
        public async static System.Threading.Tasks.Task<bool> Get_Item(string s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_item", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получить список пользователей, которым не подойдет продукт с переданным штрикхкодом, параметры- айди пользователя и штрихкод
        public async static System.Threading.Tasks.Task<bool> Get_ScanByBarcode(List<string> s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_by_barcode", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }
                            else if (response_obj.type_response == 2)
                            {
                                c = false;
                            }

                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получить список пользователей, которым не подойдет продукт с переданным составом, параметры- айди пользователя и список-состав
        public async static System.Threading.Tasks.Task<bool> Get_ScanByList(List<string> s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_by_inglist", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }
                            else if (response_obj.type_response == 2)
                            {
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получить список любимых/историю, параметр - айди пользователя и названи таблицы (из которой будут браться данные)
        public async static System.Threading.Tasks.Task<bool> Get_UserList(List<string> s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                // 1-user id,2-table name
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_user_list", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // добавление продукта в любимые, список из айди аккаунта и айди продукта
        public async static System.Threading.Tasks.Task<bool> Upload_Favourite(List<string> s)
        {if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("upload_favourite", JsonConvert.SerializeObject(s)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка добавления. Возможно, этот продукт уже есть у вас в любимых.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получение результатов поискового запроса, параметр-поисковой запрос
        public async static System.Threading.Tasks.Task<bool> Get_Search(string s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_search", s));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получение данных аккаунта по его айди, параметр- айди аккаунта
        public async static System.Threading.Tasks.Task<bool> GetData_UserById(string s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_user_id", s));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // получение пользователей аккаунта, параметр - айди аккаунта
        public async static System.Threading.Tasks.Task<bool> GetData_UserProfiles(string s)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", new RequestClass("get_user_profiles", s));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения данных.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения данных.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // удалить часть данных о пользователе, параметр - профиль с удаляемыми данными
        public async static System.Threading.Tasks.Task<bool> DeleteData_ProfilePart(AllergyProfile profile)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("delete_profile_part", JsonConvert.SerializeObject(profile)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка удаления";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка удаления";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // обновить данные о пользователе. параметр - профиль пользователя
        public async static System.Threading.Tasks.Task<bool> UpdateData_Profile(AllergyProfile profile)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("update_profile", JsonConvert.SerializeObject(profile)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка обновления";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка обновления";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        // удалить данные о пользователе, параметр - удаляемый профиль
        public async static System.Threading.Tasks.Task<bool> DeleteData_Profile(AllergyProfile profile)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("delete_profile", JsonConvert.SerializeObject(profile)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка удаления.";
                                c = false;
                            }


                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка удаления.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        //создать новый профиль, параметр-создаваемый профиль
        public async static System.Threading.Tasks.Task<bool> UploadData_NewProfile(AllergyProfile profile)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                bool c = true;
                SQLF_answer_string = "Ошибка подключения";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("UploadData", new RequestClass("upload_profile", JsonConvert.SerializeObject(profile)));
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;
                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка создания нового профиля";
                                c = false;
                            }
                            else if (response_obj.type_response == 2)
                            {
                                SQLF_answer_string = response_obj.content_response;
                                c = false;
                            }

                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка создания нового профиля";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }

        //получить айди элемента из таблицы tablename у которого переменная namevariable имеет значение variablevalue
        public async static System.Threading.Tasks.Task<bool> GetData_Id(string nametable,string namevariable,string variablevalue)
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                SQLF_answer_string = "Ошибка подключения";
                bool c = true;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        client.BaseAddress = new Uri(ConnectionString);
                        List<string> strings = new List<string>() { nametable, namevariable, variablevalue };
                        var request = new RequestClass("get_idbyname", JsonConvert.SerializeObject(strings));
                        HttpResponseMessage response = await client.PostAsJsonAsync("GetData", request);
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            var response_obj = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            SQLF_answer_string = response_obj.content_response;

                            if (response_obj.type_response == 0)
                            {
                                SQLF_answer_string = "Ошибка получения АйДи.";
                                c = false;
                            }

                        }
                    }
                }
                catch
                {
                    SQLF_answer_string = "Ошибка получения АйДи.";
                    c = false;
                }
                MainActivity.IsRunning = true;
                return c;
            }
            else
            {
                SQLF_answer_string = "Другой запрос в процессе.";
                return false;
            }
        }
    }
}