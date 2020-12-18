using System;
using System.Security.Cryptography;
using System.Text;

namespace HotbitApi.Utility
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string Create(string str, int code)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                var r = strResult.Replace("-", "");
                if (code == 16)
                {
                    return r.Substring(8, 16);
                }
                else
                {
                    return r;
                }
            }

            //string strEncrypt = string.Empty;
            //if (code == 16)
            //{
            //    strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").Substring(8, 16);
            //}

            //if (code == 32)
            //{
            //    strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            //}

            //return strEncrypt;

        }

        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string randomString(int size = 8)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //var size = (length == 0) ? 8 : length;
            var i = 1;
            var ret = "";
            Random r = new Random(System.DateTime.Now.Millisecond);
            while (i <= size)
            {
                var max = chars.Length - 1;
                //var num = Math.Floor(r.Next() * max);
                var num = r.Next(0, max);
                var temp = chars.Substring(num, 1);
                ret += temp;
                i++;
            }
            return ret;
        }
    }
}
