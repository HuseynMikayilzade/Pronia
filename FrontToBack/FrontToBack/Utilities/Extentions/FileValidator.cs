using FrontToBack.Models;

namespace FrontToBack.Utilities.Extention
{
    public static class FileValidator
    {
        public static bool CheckType(this IFormFile file,string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckSize(this IFormFile file,int mb)
        {
            return file.Length > mb * 1024 * 1024;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file,string root,params string[] folders)
        {
            string filname= Guid.NewGuid().ToString() + file.FileName ;
            string path = root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path=Path.Combine(path,filname);
            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filname;
        }
        public static async void DeleteFile(this string filename,string root , params string[]  folders)
        {
            string path = root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine (path, folders[i]);
            }
            path=Path.Combine(path,filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
