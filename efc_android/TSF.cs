using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace efc_final
{
 
    // функции, отвечающие за чтение текста/распознавание штрихкода
    public class TSF
    {
        public static string stringres = "вы не подключены к интернету";
        public static ResponseClass TSF_response;

        // распознание текста, параметр - картинка, с которой считываем текст
        public async static System.Threading.Tasks.Task DetectText(Android.Graphics.Bitmap bitmap) 
        {
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                stringres = "вы не подключены к интернету";
                try
                {
                    var b = Item.BitmapToByte(bitmap);
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        var obj = new RequestClass("detecttext", JsonConvert.SerializeObject(b));
                        client.BaseAddress = new Uri(SQLF.ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("Index", obj);
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            TSF_response = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            stringres = TSF_response.content_response;

                        }
                        
                    }
                }
                catch{}
                MainActivity.IsRunning = true;
            }

        }

        // распознание штрихкода, параметр - картинка, с которой считываем  штрихкод
        public static async System.Threading.Tasks.Task ReadBarcode(Android.Graphics.Bitmap bitmap) 
        {   
            if (MainActivity.IsRunning)
            {
                MainActivity.IsRunning = false;
                stringres = "вы не подключены к интернету";
                try
                {

                    var b = Item.BitmapToByte(bitmap);
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(20);
                        var obj = new RequestClass("readbarcode", JsonConvert.SerializeObject(b));
                        client.BaseAddress = new Uri(SQLF.ConnectionString);
                        HttpResponseMessage response = await client.PostAsJsonAsync("Index", obj);
                        if (response.IsSuccessStatusCode)
                        {
                            var stringresjson = await response.Content.ReadAsStringAsync();
                            TSF_response = JsonConvert.DeserializeObject<ResponseClass>(stringresjson);
                            stringres = TSF_response.content_response;
                        }
                        
                    }
                }
                catch{}
                MainActivity.IsRunning = true;
            }
        }

    }
}