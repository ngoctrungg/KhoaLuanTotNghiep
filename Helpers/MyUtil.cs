using System.Security.Cryptography;
using System.Text;

namespace KLTN_E.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }catch(Exception ex) 
            {
                return string.Empty;
            }
        }

        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM!@#$%^&*";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
        public static bool verifyMd5Hash(string input, string hash, string salt)
        {
            string hashOfInput = ComputeMd5Hash(input, hash);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }

        private static string ComputeMd5Hash(string input, string salt)
        {
            string data = input + salt;

            using(MD5 md5Hash =  MD5.Create())
            {
                byte[] hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string ComputeSha256Hash(string input)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < bytes.Length ; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
