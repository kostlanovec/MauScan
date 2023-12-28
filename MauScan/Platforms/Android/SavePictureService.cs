using Android.Content;
using Android.Provider;

namespace MauScan.Platforms.Android
{
    public static class SavePictureService
    {
        public static bool SavePicture(byte[] arr, string imageName)
        {
            var contentValues = new ContentValues();
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, imageName);
            contentValues.Put(MediaStore.Files.IFileColumns.MimeType, "image/png");
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, "Pictures/relativePath");
            try
            {
                var uri = MainActivity.Instance.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);
                var output = MainActivity.Instance.ContentResolver.OpenOutputStream(uri);
                output.Write(arr, 0, arr.Length);
                output.Flush();
                output.Close();
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
            contentValues.Put(MediaStore.IMediaColumns.IsPending, 1);
            return true;
        }
    }
}
