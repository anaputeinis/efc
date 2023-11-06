using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing;
using Tesseract;
using SkiaSharp;

namespace web_thingy.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        // Onpost:
        // получает RequestClass, где type_request - тип операции, которую нужно совершить, content_request - данные, необходимые для совершения указанной операции
        // возвращает ResponseClass, где type_response - успешность выполнения (0- не успешно, 1- успешно и 2- вариативный случай), content_response- возвращаемые данные или сообщение об ошибке
        public async Task<IActionResult> OnPost()
        {
            string raw_body = await Helpers.ExtractBodyAsync(this.Request.Body);
            var v = JsonConvert.DeserializeObject<RequestClass>(raw_body);
            string text = "символы не были распознаны";
            int type_response = 1;
            try
            {
                var b = JsonConvert.DeserializeObject<byte[]>(v.content_request);
                if (b != null)
                {
                    if (v?.type_request == "detecttext") { text = DetectText(b); }
                    else if (v?.type_request == "readbarcode") { text = ReadBarcode(b); }
                    else { type_response = 0; }
                }
                else
                {
                    type_response = 0;
                }
                if (text == "символы не были распознаны")
                {
                    type_response = 0;
                }
            }
            catch { type_response = 0; text = "Ошибка работы сервера.";}
            ResponseClass response = new ResponseClass(type_response, text);
            
            return Content(JsonConvert.SerializeObject(response));
        }

        // считывание штрихкода, параметры - фотография, возвращает строку с штрихкодом
        public string ReadBarcode(byte[] bytearray)
        {
            SKBitmap bitMap = SkiaSharp.SKBitmap.Decode(bytearray);
            var reader = new ZXing.SkiaSharp.BarcodeReader();
            var result = reader.Decode(bitMap);
            string res;
            if (result!=null)
            { 
                res = result.Text; 
            }
            else
            {
                res = "символы не были распознаны";
            }
            return res;
        }

        // распознавание теста, параметры - фотография, возвращает строку с текстом
        public string DetectText(byte[] bytearray)
        {
            string result;
            using (var engine = new TesseractEngine("D:\\vs\\web_thingy\\bin\\Debug\\tessdata", "rus", EngineMode.Default))
            {
                using (Pix img = PixConverter.ToPix(ByteArrayToBitmap(bytearray)))
                {
                    using (var page = engine.Process(img))
                    {
                        result = page.GetText();
                        if (result.Length<=3) { result = "символы не были распознаны"; }
                    }
                }
            }
            return result;
        }

        //конвертирует byte[] в bitmap 
        public Bitmap ByteArrayToBitmap(byte[] byteBuffer)
        {
            Bitmap bmpReturn = null;

            MemoryStream memoryStream = new MemoryStream(byteBuffer);


            memoryStream.Position = 0;


            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);


            memoryStream.Close();

            return bmpReturn;
        }
    }
}