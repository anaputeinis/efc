using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.AppCompat.App;
using Com.Theartofdev.Edmodo.Cropper;
using Xamarin.Essentials;


namespace efc_final
{
    [Activity(Label = "CameraPage", NoHistory = false)]
    public class CameraPage : AppCompatActivity
    {
        // CurrentImage - фотография, которая будет передана для чтения  штрихкода/распознавания текста

        private Button ScanBarCodeButton, ReadTextButton, SeeDetectedTextButton, ExitButton, InputByHandButton, CameraButton, GalleryButton, CropButton,RotateButton;
        public string TextResult;
        public static string TextResultRedacted;
        public CropImageView CropView;

        private Dialog see_text_dialog;
        private EditText SeeDetectedText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.camera_page_layout);

            CameraButton = FindViewById<Button>(Resource.Id.open_camera_button);
            GalleryButton = FindViewById<Button>(Resource.Id.choose_from_gallery_button);
            CropButton = FindViewById<Button>(Resource.Id.crop_button);
            
            ScanBarCodeButton = FindViewById<Button>(Resource.Id.camera_scan_barcode_button);
            ReadTextButton = FindViewById<Button>(Resource.Id.camera_read_text_button);
            InputByHandButton = FindViewById<Button>(Resource.Id.camera_input_by_hand_button);
            CropView = FindViewById<CropImageView>(Resource.Id.cropImageView);
            RotateButton = FindViewById<Button>(Resource.Id.camera_page_rotate_button);

            CameraButton.Click += CameraButton_Click;
            GalleryButton.Click += GalleryButton_Click;
            CropButton.Click += CropButton_Click;
            RotateButton.Click += RotateButton_Click;
            InputByHandButton.Click += InputByHandButton_Click;

            ReadTextButton.Click += ReadTextButton_Click;
            ScanBarCodeButton.Click += ScanBarCodeButton_Click;

        }

        // поворот сделанной фотографии
        private void RotateButton_Click(object sender, EventArgs e)
        {
            if (CropView.GetCroppedImage(500, 500) !=null)
            {
                CropView.RotateImage(90);
            }
        }

        // ввести состав/штрихкод вручную
        private void InputByHandButton_Click(object sender, EventArgs e)
        {
            see_text_dialog = new Dialog(this);
            see_text_dialog.SetContentView(Resource.Layout.see_detected_text_popup);
            see_text_dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            see_text_dialog.Show();

            SeeDetectedText = see_text_dialog.FindViewById<EditText>(Resource.Id.see_detected_text);
            SeeDetectedTextButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_button);
            ExitButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_exit_button);
            SeeDetectedText.Hint=String.Empty;

            ExitButton.Click += ExitButton_Click;
            SeeDetectedTextButton.Click += SeeDetectedTextButton_Click;
        }


        // получение CurrentImage из галереи
        async void GalleryButton_Click(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                var path = photo.FullPath;
                var b = BitmapFactory.DecodeFile(path);
                CropView.SetImageBitmap(b);

                ScanBarCodeButton.Visibility = ViewStates.Visible;
                ReadTextButton.Visibility = ViewStates.Visible;
            }
            catch { return; }

        }

        // получение CurrentImage из камеры
        async void CameraButton_Click(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"xamarin.{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png"
                });

                var path = photo.FullPath;
                var b = BitmapFactory.DecodeFile(path);
                CropView.SetImageBitmap(b);

                ScanBarCodeButton.Visibility = ViewStates.Visible;
                ReadTextButton.Visibility = ViewStates.Visible;
            }
            catch { return; }
        }


        // обрезка полученной фотографии
        private void CropButton_Click(object sender, EventArgs e)
        {

            var cropped = CropView.GetCroppedImage(500, 500);
            if (cropped != null)
            {
                CropView.SetImageBitmap(cropped);
            }
        }

        // переданное пользователем изображение передается в веб приложение для сканирования штрихкода
        public async void ScanBarCodeButton_Click(object sender, EventArgs e)
        {
            see_text_dialog = new Dialog(this);
            see_text_dialog.SetContentView(Resource.Layout.see_detected_text_popup);
            see_text_dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            see_text_dialog.Show();

            SeeDetectedText = see_text_dialog.FindViewById<EditText>(Resource.Id.see_detected_text);
            SeeDetectedTextButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_button);
            ExitButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_exit_button);

            if (CropView.GetCroppedImage(500, 500) != null)
            {
                await TSF.ReadBarcode(CropView.GetCroppedImage(500, 500));
                TextResult = TSF.stringres;
                SeeDetectedText.Text = TextResult;
            }

            ExitButton.Click += ExitButton_Click;
            SeeDetectedTextButton.Click += SeeDetectedTextButton_Click;

        }


        // переданное пользователем изображение передается в веб приложение для распознавания текста
        public async void ReadTextButton_Click(object sender, EventArgs e)
        {
            see_text_dialog = new Dialog(this);
            see_text_dialog.SetContentView(Resource.Layout.see_detected_text_popup);
            see_text_dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            see_text_dialog.Show();

            SeeDetectedText = see_text_dialog.FindViewById<EditText>(Resource.Id.see_detected_text);
            SeeDetectedTextButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_button);
            ExitButton = see_text_dialog.FindViewById<Button>(Resource.Id.see_detected_text_exit_button);

            if (CropView.GetCroppedImage(500, 500) != null)
            {
                await TSF.DetectText(CropView.GetCroppedImage(500, 500));
                TextResult = TSF.stringres;
                SeeDetectedText.Text = TextResult;
            }

            ExitButton.Click += ExitButton_Click;
            SeeDetectedTextButton.Click += SeeDetectedTextButton_Click;
        }

        // полученный распознанный текст/полученный штрихкод отправляется на анализ состава/поиск продукта
        private void SeeDetectedTextButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(SeeDetectedText.Text) && (SeeDetectedText.Text != "вы не подключены к интернету" && SeeDetectedText.Text != "символы не были распознаны"))
            {
                TextResultRedacted = SeeDetectedText.Text;
                StartActivity(new Android.Content.Intent(this, typeof(ItemPage_Scan)));
                Finish();

            }

        }

        // закрытие диалогового окна с распознанным текстом/полученным штрихкодом
        private void ExitButton_Click(object sender, EventArgs e)
        {
            see_text_dialog.Dismiss();
            see_text_dialog.Hide();
        }

        // запрос разрешения на использование камеры и доступа к галерее
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

}