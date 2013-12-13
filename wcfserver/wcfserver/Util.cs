using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace wcfserver
{
    internal class Util
    {
        internal static string Md5String(string stringToMd5)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] bytesOfStringToMd5 = ASCIIEncoding.ASCII.GetBytes(stringToMd5);
                byte[] bytesMd5 = md5Hash.ComputeHash(bytesOfStringToMd5, 0, bytesOfStringToMd5.Length);

                return ByteArrayToString(bytesMd5);
            }
        }

        internal static string ByteArrayToString(byte[] byteData)
        {
            StringBuilder hex = new StringBuilder(byteData.Length * 2);
            foreach (byte b in byteData)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        internal static string prettifyDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}