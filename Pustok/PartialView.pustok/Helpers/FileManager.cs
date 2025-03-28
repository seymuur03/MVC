using Microsoft.AspNetCore.Hosting;

namespace PartialView.pustok.Helpers
{
    public static class FileManager
    {
        public static string SaveImage(this IFormFile formFile,string path,string folder)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
            string Fpath = Path.Combine(path, folder, fileName);
            using FileStream fileStream = new FileStream(Fpath, FileMode.Create);
            formFile.CopyTo(fileStream);
            return fileName;
        }
        //public static bool IsCorrectSize(this IFormFile formFile,int size)
        //{
        //    return formFile.Length>=size;
        //}
        //public static bool IsCorrectType(this IFormFile formFile, string[] fileTypes)
        //{
        //    return fileTypes.Contains(formFile.ContentType);
        //}

        public static bool DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
                
        }
    }
}
