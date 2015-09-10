namespace TheKnot.Membership.Security.HelperClasses
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public sealed class CryptoHelper
    {
        private static byte[] IV_192 = new byte[] { 0x21, 0x36, 0x6c, 0x9b, 0xcc, 0xde, 0x31, 0x76 };
        private static byte[] KEY_192 = new byte[] { 0xde, 0x42, 0xdb, 0x76, 0x2c, 0x87, 0x48, 0x66 };
        private static byte[] KEY_HMAC = new byte[] { 
            0x4b, 0x4c, 0x41, 0x53, 0x4a, 0x44, 70, 0x4c, 0x55, 0x33, 0x34, 0x38, 0x37, 0x45, 0x52, 0x48, 
            0x4a, 0x38, 0x39, 0x47, 0x59, 50, 0x34, 0x4a, 0x49, 0x4f, 80, 0x51, 70, 0x4a, 0x43, 0x37, 
            0x39, 0x30, 0x33, 0x34, 0x39, 0x30, 0x56, 0x33, 0x34, 0x39, 0x30, 0x56, 0x4d, 0x48, 0x47, 0x4a, 
            0x30, 0x44, 0x4b, 0x58, 0x57, 0x33, 0x30, 0x48, 0x47, 0x43, 50, 0x39, 0x30, 0x43, 0x59, 0x35
         };

        private CryptoHelper()
        {
        }

        public static string DecryptDes(string input)
        {
            byte[] buffer;
            try
            {
                buffer = Convert.FromBase64String(input);
            }
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
            catch (FormatException)
            {
                return string.Empty;
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = null;
            CryptoStream stream2 = null;
            StreamReader reader = null;
            string str = string.Empty;
            try
            {
                stream = new MemoryStream(buffer);
                stream2 = new CryptoStream(stream, provider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
                reader = new StreamReader(stream2);
                str = reader.ReadLine();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (stream2 != null)
                {
                    stream2.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return str;
        }

        public static string DecryptDesFromWeb(string input)
        {
            if ((input != null) && (input.Length != 0))
            {
                return DecryptDes(HttpUtility.UrlDecode(input));
            }
            return string.Empty;
        }

        public static string EncryptDes(string input)
        {
            if ((input == null) || (input.Length == 0))
            {
                return string.Empty;
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream stream = null;
            CryptoStream stream2 = null;
            StreamWriter writer = null;
            string str = string.Empty;
            try
            {
                stream = new MemoryStream();
                stream2 = new CryptoStream(stream, provider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
                writer = new StreamWriter(stream2);
                writer.WriteLine(input);
                writer.Flush();
                stream2.FlushFinalBlock();
                stream.Flush();
                str = Convert.ToBase64String(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length, CultureInfo.InvariantCulture));
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (stream2 != null)
                {
                    stream2.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return str;
        }

        public static string EncryptDesForWeb(string input)
        {
            if ((input != null) && (input.Length != 0))
            {
                return HttpUtility.UrlEncode(EncryptDes(input));
            }
            return string.Empty;
        }

        public static string EncryptHmacSha1(string input)
        {
            if ((input == null) || (input.Length == 0))
            {
                return string.Empty;
            }
            byte[] bytes = new ASCIIEncoding().GetBytes(input);
            byte[] buffer2 = null;
            using (HMACSHA1 hmacsha = new HMACSHA1(KEY_HMAC))
            {
                buffer2 = hmacsha.ComputeHash(bytes);
            }
            return GetAsHexaDecimal(buffer2);
        }

        private static string GetAsHexaDecimal(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            int length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                builder.Append(string.Format(CultureInfo.InvariantCulture, "{0,2:x}", new object[] { bytes[i] }).Replace(" ", "0"));
            }
            return builder.ToString();
        }
    }
}

