﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;
using Android.Graphics;
using Uri = Android.Net.Uri;
using System.IO;
using Java.IO;

namespace Image_Manipulator
{
    [Activity(Label = "Image Manipulator", MainLauncher = true, Icon = "@mipmap/icon")]

    public class MainActivity : Activity
    {
        void copy_bitmap(Bitmap original, Bitmap copy)
        {
            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    int p = original.GetPixel(i, j);
                    Color c = new Color(p);
                    copy.SetPixel(i, j, c);
                }
            }
        }

        
            





        public static readonly int PickImageId = 1000;
        public static readonly int TakeImageId = 2000;
        string filepath;
        public static Java.IO.File _file;
        public static Java.IO.File _dir;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button btn_load_gallery = FindViewById<Button>(Resource.Id.btn_load_gallery);
            btn_load_gallery.Click += delegate
            {
                //load_gallery();
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            };

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                FindViewById<Button>(Resource.Id.btn_take_picture).Click += TakePicture;
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities
                (intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void CreateDirectoryForPictures()
        {
            _dir = new Java.IO.File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraExample");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid()));
            filepath = _file.AbsolutePath;
            StartActivityForResult(intent, TakeImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            
            //take picture
            if ((requestCode == TakeImageId) && (resultCode == Result.Ok) && (data != null))
            {
                base.OnActivityResult(requestCode, resultCode, data);
                Bitmap bitmap = (Bitmap)data.Extras.Get("data");

                var to_send = new Intent(this, typeof(ViewImage));
                to_send.PutExtra("image", bitmap);
                StartActivity(to_send);
            }

            //load from gallery
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Uri uri = data.Data;
                //_imageView.SetImageURI(uri);
                //Android.Graphics.Bitmap gallery_bitmap = (Android.Graphics.Bitmap)data.Extras.Get("data");






                //Stream stream = ContentResolver.OpenInputStream(data.Data);
                //imageView1.SetImageBitmap(DecodeBitmapFromStream(data.Data, 150, 150));
                //Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                //MemoryStream memStream = new MemoryStream();
                //you can change 60 (100 big size reduce to less >>>>
                //bitmap.Compress(Bitmap.CompressFormat.Jpeg, 60, memStream);



                
                Bitmap bitmap = null;
                bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uri);


                //Bitmap bitmap_copy = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, bitmap.GetConfig());
                //copy_bitmap(bitmap, bitmap_copy); 

                var to_send = new Intent(this, typeof(ViewImage));
                to_send.PutExtra("image", bitmap); //gallery bitmap should be uri? not sure
                StartActivity(to_send);
            }




            //Make image available in the gallery
            /*
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            var contentUri = Android.Net.Uri.FromFile(_file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            */


        }
    }
}

