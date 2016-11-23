using Android.App;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Content;
using Android.Support.V7.App;
using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using System;
using Android.Support.V4.App;
using Android.Views;
using Android.Provider;
using Android.Runtime;
using Android.Graphics;

namespace XamarinCropImage
{
    [Activity(Label = "XamarinCropImage", MainLauncher = true, Icon = "@drawable/icon",Theme ="@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        ImageView imageView;
        Android.Support.V7.Widget.Toolbar toolbar;
        File file;
        Android.Net.Uri uri;
        Intent CamIntent, GalIntent, CropIntent;
        const int RequestPermissionCode = 1;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Crop Image";
            toolbar.SetTitleTextColor(Android.Graphics.Color.White);
            SetSupportActionBar(toolbar);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);
            int permissionCheck = (int)ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
            if (permissionCheck == (int)Permission.Denied)
                RequestRuntimePermission();
                
        }

        private void RequestRuntimePermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
                Toast.MakeText(this, "CAMERA permission will allows us to access CAMERA app", ToastLength.Short).Show();
            else
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, RequestPermissionCode);

        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.btn_camera)
                CameraOpen();
            else if (item.ItemId == Resource.Id.btn_gallery)
                GalleryOpen();
            return true;
        }

        private void GalleryOpen()
        {
            GalIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from Gallery"), 2);
        }

        private void CameraOpen()
        {
            CamIntent = new Intent(MediaStore.ActionImageCapture);
            file = new File(Android.OS.Environment.ExternalStorageDirectory, "file_" + Guid.NewGuid().ToString() + ".jpg");
            uri = Android.Net.Uri.FromFile(file);
            CamIntent.PutExtra(MediaStore.ExtraOutput, uri);
            CamIntent.PutExtra("return-data", true);
            StartActivityForResult(CamIntent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 0 && resultCode == Result.Ok)
                CropImage();
            else if (requestCode == 2)
            {
                if (data != null)
                {
                    uri = data.Data; 
                    CropImage();
                }
            }
            else if(requestCode == 1)
            {
                if(data != null)
                {
                    Bundle bundle = data.Extras;
                    Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");
                    imageView.SetImageBitmap(bitmap);
                }
            }
        }

        private void CropImage()
        {
           try
            {
                CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(uri, "image/*");

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 180);
                CropIntent.PutExtra("outputY", 180);
                CropIntent.PutExtra("aspectX", 3);
                CropIntent.PutExtra("aspectY", 4);
                CropIntent.PutExtra("scaleUpIfNeeded",true);
                CropIntent.PutExtra("return-data", true);

                StartActivityForResult(CropIntent, 1);
            }
            catch(ActivityNotFoundException ex)
            {

            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
           switch(requestCode)
            {
                case RequestPermissionCode:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                            Toast.MakeText(this, "Permission Granted", ToastLength.Short).Show();
                        else
                            Toast.MakeText(this, "Permission Canceled", ToastLength.Short).Show();
                    }
                    break;
            }
        }
    }
}

