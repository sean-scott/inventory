using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Provider;
using Inventory.Interfaces;
using Inventory.Model;
using Inventory.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Droid
{
    /// <summary>
    /// Android-specific helper methods and fields that can be used across the application.
    /// </summary>
    public static class AndroidHelper
    {
        public static int IS_NEW = 0;
        public static int IS_VIEWING = 1;
        public static int IS_EDITING = 2;
        public static int IS_PRODUCT = 3;
        public static int IS_BATCH = 4;
        public static int RESULT_PRODUCT_DEFAULTS = 978;
        
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
        
        /// <summary>
        /// Saves image to directory specified by App._dir
        /// </summary>
        public static void StoreImageToAppDirectory(Bitmap bitmap, string filepath)
        {
            System.Diagnostics.Debug.WriteLine("AndroidHelper.StoreImageToAppDirectory()");

            FileStream fs = null;
            DeleteImageFromDirectory(filepath);

            fs = File.Create(filepath);

            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 25, fs);

            fs.Close();
        }

        public static void DeleteImageFromDirectory(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
    }
}