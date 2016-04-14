using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Debonair.Utilities.Extensions
{
    public static class StringExts
    {
        public static string ToMd5Hash(this string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var item in hash)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }

        public static bool IsNumeric(this string value)
        {
            return new Regex(@"^\d+$").Match(value).Success;
        }

        public static string ToTitleCase(this string value)
        {
            try
            {
                //Convert to lower as ToTitleCase doesnt work on uppercase
                return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
            }
            catch
            { return value; }
        }

        public static string RemoveAccent(this string value)
        {
            return Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(value));
        }

        public static string ToUrlFormat(this string value)
        {
            var str = value.RemoveAccent().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
            return str;
        }

        public static TEntity ToType<TEntity>(this string value)
        {
            return (TEntity)Convert.ChangeType(value, typeof(TEntity));
        }
    }
}
