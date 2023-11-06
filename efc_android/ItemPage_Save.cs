using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Com.Theartofdev.Edmodo.Cropper;

namespace efc_final
{
    [Activity(Label = "ItemPage_Save", NoHistory = false)]
    public class ItemPage_Save : Activity
    {
        private Button Save_CameraButton,Save_GalleryButton,Save_ScanBarcodeButton,
            Save_DeleteButton,Save_SaveButton,Save_AddButton,Save_DeleteIngrButton, Save_RotateButton;
        private EditText Save_Name,Save_Barcode,Save_Ingredient;
        private TextView Save_Warning, Save_Instruction, Blank1,Blank4, Blank5, Save_InstructionList;
        private ListView Save_ListView;
        private CheckBox Save_CheckBox_gluten, Save_CheckBox_lactose;
        List<List<string>> Ingredients_Edit=new List<List<string>>();
        AdapterPlain save_adapter;
        private CropImageView Save_Image;
        private Int64 Barcode;

        private Android.Graphics.Bitmap Save_Bitmap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.item_page_layout);
            Save_CameraButton = FindViewById<Button>(Resource.Id.item_camera_button);
            Save_GalleryButton = FindViewById<Button>(Resource.Id.item_gallery_button);
            Save_ScanBarcodeButton = FindViewById<Button>(Resource.Id.item_barcode_button);
            Save_DeleteButton = FindViewById<Button>(Resource.Id.item_delete_button);
            Save_SaveButton = FindViewById<Button>(Resource.Id.item_save_button);
            Save_Name = FindViewById<EditText>(Resource.Id.item_name_edit);
            Save_Barcode = FindViewById<EditText>(Resource.Id.item_qr_edit);
            Save_ListView = FindViewById<ListView>(Resource.Id.item_listview);
            Save_Image = FindViewById<CropImageView>(Resource.Id.item_image);
            Save_Warning = FindViewById<TextView>(Resource.Id.item_page_warning);
            Save_Instruction = FindViewById<TextView>(Resource.Id.item_checkbox_instruction);
            Save_CheckBox_gluten = FindViewById<CheckBox>(Resource.Id.item_checkBox_gluten);
            Save_CheckBox_lactose = FindViewById<CheckBox>(Resource.Id.item_checkBox_lactose);
            Save_Ingredient = FindViewById<EditText>(Resource.Id.item_add_text);
            Save_AddButton = FindViewById<Button>(Resource.Id.item_add_button);
            Save_DeleteIngrButton = FindViewById<Button>(Resource.Id.item_delete_ingredient_button);
            Save_RotateButton= FindViewById<Button>(Resource.Id.item_rotate_button);
            Save_InstructionList = FindViewById<TextView>(Resource.Id.item_list_instruction);


            Blank1 = FindViewById<TextView>(Resource.Id.item_buttons_1);
            Blank4 = FindViewById<TextView>(Resource.Id.item_buttons_4);
            Blank5 = FindViewById<TextView>(Resource.Id.item_buttons_5);

            Blank1.Visibility = ViewStates.Invisible;
            Blank4.Visibility=ViewStates.Invisible;
            Blank5.Visibility=ViewStates.Invisible;




            Save_CameraButton.Visibility = ViewStates.Visible;
            Save_GalleryButton.Visibility = ViewStates.Visible;
            Save_ScanBarcodeButton.Visibility = ViewStates.Visible;
            Save_DeleteButton.Visibility = ViewStates.Visible;
            Save_SaveButton.Visibility = ViewStates.Visible;
            Save_Name.Visibility = ViewStates.Visible;
            Save_ListView.Visibility = ViewStates.Visible;
            Save_Barcode.Visibility = ViewStates.Visible;
            Save_Image.Visibility = ViewStates.Visible;
            Save_Instruction.Visibility = ViewStates.Visible;
            Save_CheckBox_lactose.Visibility = ViewStates.Visible;
            Save_Ingredient.Visibility = ViewStates.Visible;
            Save_CheckBox_gluten.Visibility = ViewStates.Visible;
            Save_AddButton.Visibility = ViewStates.Visible;
            Save_DeleteIngrButton.Visibility = ViewStates.Visible;
            Save_RotateButton.Visibility = ViewStates.Visible;
            Save_InstructionList.Visibility = ViewStates.Visible;

            Save_InstructionList.Text = "Нажмите на ингредиент, чтобы отредактировать";

            Save_CameraButton.Click += Save_CameraButton_Click;
            Save_GalleryButton.Click += Save_GalleryButton_Click;
            Save_SaveButton.Click += Save_SaveButton_Click;
            Save_ScanBarcodeButton.Click += Save_ScanBarcodeButton_Click;
            Save_AddButton.Click += Save_AddButton_Click;
            Save_DeleteIngrButton.Click += Save_DeleteIngrButton_Click;
            Save_RotateButton.Click += Save_RotateButton_Click;
            Save_DeleteButton.Click += Save_DeleteButton_Click;
            Save_ScanBarcodeButton.Text = "Сканировать штрихкод";

            var check = Int64.TryParse(CameraPage.TextResultRedacted, out Barcode);
            if (!check)
            {


                foreach (var el in Item.Ingredients_Normalize(CameraPage.TextResultRedacted))
                {
                    Ingredients_Edit.Add(new List<string>() { el, "0", "0" });
                }
            }
            else 
            {
                Save_Barcode.Text=Barcode.ToString();
            }
            save_adapter = new AdapterPlain(this, Ingredients_Edit);
            Save_ListView.Adapter = save_adapter;

            Save_ListView.ItemLongClick += Save_ListView_ItemLongClick;
        }

        // отмена сохранения товара, возврат на главную страницу
        private void Save_DeleteButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
            Finish();
        }

        //повернуть сделанное изображение
        private void Save_RotateButton_Click(object sender, EventArgs e)
        {
            
            if (Save_Image.GetCroppedImage(500, 500)!=null)
            {
                Save_Image.RotateImage(90);
            }
        }

        // удалить выбранный ингредиент
        private void Save_DeleteIngrButton_Click(object sender, EventArgs e)
        {
            Save_Ingredient.Text=String.Empty;
            Save_CheckBox_lactose.Checked = false;
            Save_CheckBox_gluten.Checked = false;
        }

        // редактировать ингредиент при долгом нажатии на него
        private void Save_ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            Save_Ingredient.Text = Ingredients_Edit[e.Position][0].ToString();

            if (Ingredients_Edit[e.Position][1] == "1") 
            {
                Save_CheckBox_gluten.Checked=true;
            }
            if (Ingredients_Edit[e.Position][2] == "1")
            {
                Save_CheckBox_lactose.Checked = true;
            }

            Ingredients_Edit.Remove(Ingredients_Edit[e.Position]);
            save_adapter.NotifyDataSetChanged();
        }

        //сохранить ингредиент
        private void Save_AddButton_Click(object sender, EventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(Save_Ingredient.Text))
            {
                List<string> s = new List<string>();

                string g = "0";
                string l = "0";

                if (Save_CheckBox_gluten.Checked)
                {
                    g = "1";
                }
                if (Save_CheckBox_lactose.Checked)
                {
                    l = "1";
                }
                s = new List<string>() { Save_Ingredient.Text,g,l };
                Ingredients_Edit.Add(s);
                save_adapter.NotifyDataSetChanged();
                Save_CheckBox_lactose.Checked = false;
                Save_CheckBox_gluten.Checked = false;
                Save_Ingredient.Text = String.Empty;
            }
            else 
            {
                Save_Warning.Visibility = ViewStates.Visible;
                Save_Warning.Text = "Введите данные";
            }
        }

        // получение штрихкода из камеры
        private async void Save_ScanBarcodeButton_Click(object sender, EventArgs e)
        {
            if (Save_Image.GetCroppedImage(500, 500)!= null)
            {
                Save_Barcode.Text = "пожалуйста, подождите";
                Save_Bitmap = Save_Image.GetCroppedImage(500, 500);
                await TSF.ReadBarcode(Save_Bitmap);
                Save_Barcode.Text = TSF.stringres;
            }

        }

        // создание нового элемента Item (продукт)
        private async void Save_SaveButton_Click(object sender, EventArgs e)
        {
            Save_Warning.Visibility = ViewStates.Gone;
            try
            {
                Int64.Parse(Save_Barcode.Text.ToString().Trim());
            }
            catch
            {
                Save_Warning.Visibility = ViewStates.Visible;
                Save_Warning.Text = "Штрихкод должен состоять из цифр.";
                return;
            }

            if (!String.IsNullOrWhiteSpace(Save_Name.Text) && !String.IsNullOrWhiteSpace(Save_Barcode.Text) && Ingredients_Edit.Count != 0)
            {
                Save_Warning.Visibility = ViewStates.Visible;
                Save_Warning.Text = "пожалуйста, подождите";
                Item item = new Item(Save_Name.Text, Ingredients_Edit, Int64.Parse(Save_Barcode.Text));
                item.SeeIfContains_Item();


                if (await ItemPage_SaveNewItem(item))
                {
                    StartActivity(new Android.Content.Intent(this, typeof(DashBoard)));
                    Finish();
                }

            }
            else 
            {
                Save_Warning.Visibility = ViewStates.Visible;
                Save_Warning.Text = "Пожалуйста, введите все данные.";
            }
            
        }


        // сохранение Item (продукт) на SQL сервере, параметр - сохраняемый элемент
        private async Task<bool> ItemPage_SaveNewItem(Item item) 
        {
            bool c = true;
            Save_Warning.Visibility = ViewStates.Gone;
            try 
            {
                if (!await SQLF.UploadData_NewItem(item)) { c = false; Save_Warning.Visibility = ViewStates.Visible; Save_Warning.Text = SQLF.SQLF_answer_string; }
                else { item.Id = Int32.Parse(SQLF.SQLF_answer_string); }
              
            }
            catch 
            {
                Save_Warning.Text = SQLF.SQLF_answer_string;
                c= false;
            }

            return c;
        }

        // выбор фото продукта из галереи
        private async void Save_GalleryButton_Click(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                var path = photo.FullPath;
                var b = BitmapFactory.DecodeFile(path);
                Save_Image.SetImageBitmap(b);
                Save_Bitmap = b;
            }
            catch { return; }

        }

        // выбор фото продукта из камеры
        private async void Save_CameraButton_Click(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"xamarin.{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png"
                });
                var path = photo.FullPath;
                var b = BitmapFactory.DecodeFile(path);
                Save_Image.SetImageBitmap(b);
                Save_Bitmap = b;
            }
            catch { return; }
        }
    }
}