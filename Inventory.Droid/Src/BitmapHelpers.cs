using Android.Graphics;

namespace Inventory.Droid
{
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string filename, int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("BitmapHelpers.LoadAndResizeBitmap()");
            System.Diagnostics.Debug.WriteLine("Filename: " + filename);
            System.Diagnostics.Debug.WriteLine("Width: " + width);
            System.Diagnostics.Debug.WriteLine("Height: " + height);

            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(filename, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            System.Diagnostics.Debug.WriteLine("outWidth: " + outWidth);
            System.Diagnostics.Debug.WriteLine("outHeight: " + outHeight);

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(filename, options);

            return resizedBitmap;
        }
    }
}