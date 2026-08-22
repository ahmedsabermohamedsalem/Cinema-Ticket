namespace Cinema_Ticket.services
{
    public class Imag
    {



        public   static void deletefile(string oldFileName)
        {
            if (!string.IsNullOrEmpty(oldFileName))
            {
                var oldFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploadsFile",
                    oldFileName
                );

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

        }

        public static string Createfile(IFormFile file)
        {

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot", "uploadsFile"
            );


            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;


        }
    }
}