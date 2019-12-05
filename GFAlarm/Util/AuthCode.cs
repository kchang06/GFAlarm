using ICSharpCode.SharpZipLib.GZip;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GFAlarm.Util
{
    public class AuthCode
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 복호화 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decode(string source, string key)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(key))
                return "";

            if (source.Length == 1)
                return source;

            try
            {
                key = MD5(key);
                string str = MD5(CutString(key, 16, 16));
                string text = MD5(CutString(key, 0, 16));
                string pass = str + MD5(str);
                if (source.StartsWith("#"))
                {
                    source = source.Substring(1);
                    byte[] input;
                    try
                    {
                        input = Convert.FromBase64String(CutString(source, 0));
                    }
                    catch
                    {
                        try
                        {
                            input = Convert.FromBase64String(CutString(source + "=", 0));
                        }
                        catch
                        {
                            try
                            {
                                input = Convert.FromBase64String(CutString(source + "==", 0));
                            }
                            catch
                            {
                                return "";
                            }
                        }
                    }
                    byte[] array = RC4(input, pass);
                    string @string = encoding.GetString(array);
                    long num2 = long.Parse(CutString(@string, 0, 10));
                    byte[] array2 = new byte[array.Length - 26];
                    Array.Copy(array, 26, array2, 0, array.Length - 26);
                    byte[] array3 = new byte[array.Length - 26 + text.Length];
                    Array.Copy(array2, 0, array3, 0, array2.Length);
                    Array.Copy(encoding.GetBytes(text), 0, array3, array2.Length, text.Length);
                    using (MemoryStream stream = new MemoryStream(array2))
                    {
                        using (Stream stream2 = new GZipInputStream(stream))
                        {
                            using (StreamReader reader = new StreamReader(stream2, Encoding.UTF8))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    byte[] input;
                    try
                    {
                        input = Convert.FromBase64String(CutString(source, 0));
                    }
                    catch
                    {
                        try
                        {
                            input = Convert.FromBase64String(CutString(source + "=", 0));
                        }
                        catch
                        {
                            try
                            {
                                input = Convert.FromBase64String(CutString(source + "==", 0));
                            }
                            catch
                            {
                                return "";
                            }
                        }
                    }
                    string @string = encoding.GetString(RC4(input, pass));
                    long num2 = long.Parse(CutString(@string, 0, 10));
                    return CutString(@string, 26);
                }
            }
            catch { } 
            return "";
        }

        private static string CutString(string str, int startIndex)
        {
            return AuthCode.CutString(str, startIndex, str.Length);
        }
        private static string CutString(string str, int startIndex, int length)
        {
            bool flag = startIndex >= 0;
            string result;
            if (flag)
            {
                bool flag2 = length < 0;
                if (flag2)
                {
                    length *= -1;
                    bool flag3 = startIndex - length < 0;
                    if (flag3)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex -= length;
                    }
                }
                bool flag4 = startIndex > str.Length;
                if (flag4)
                {
                    result = "";
                    return result;
                }
            }
            else
            {
                bool flag5 = length < 0;
                if (flag5)
                {
                    result = "";
                    return result;
                }
                bool flag6 = length + startIndex > 0;
                if (!flag6)
                {
                    result = "";
                    return result;
                }
                length += startIndex;
                startIndex = 0;
            }
            bool flag7 = str.Length - startIndex < length;
            if (flag7)
            {
                length = str.Length - startIndex;
            }
            result = str.Substring(startIndex, length);
            return result;
        }

        public static string MD5(string str)
        {
            byte[] bytes = AuthCode.encoding.GetBytes(str);
            return AuthCode.MD5(bytes);
        }
        public static string MD5(byte[] b)
        {
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string text = "";
            for (int i = 0; i < b.Length; i++)
            {
                text += b[i].ToString("x").PadLeft(2, '0');
            }
            return text;
        }

        private static byte[] GetKey(byte[] pass, int kLen)
        {
            byte[] array = new byte[kLen];
            for (long num = 0L; num < (long)kLen; num += 1L)
            {
                array[(int)(checked((IntPtr)num))] = (byte)num;
            }
            long num2 = 0L;
            for (long num3 = 0L; num3 < kLen; num3 += 1L)
            {
                num2 = (num2 + array[(int)num3] + pass[(int)(num3 % pass.Length)]) % kLen;
                checked
                {
                    byte b = array[(int)((IntPtr)num3)];
                    array[(int)((IntPtr)num3)] = array[(int)((IntPtr)num2)];
                    array[(int)((IntPtr)num2)] = b;
                }
            }
            return array;
        }

        public static byte[] RC4(byte[] input, string pass)
        {
            bool flag = input == null || pass == null;
            byte[] result;
            if (flag)
            {
                result = null;
            }
            else
            {
                byte[] array = new byte[input.Length];
                byte[] key = AuthCode.GetKey(AuthCode.encoding.GetBytes(pass), 256);
                long num = 0L;
                long num2 = 0L;
                for (int i = 0; i < input.Length; i += 1)
                {
                    num = (num + 1L) % key.Length;
                    num2 = (num2 + key[num]) % key.Length;
                    checked
                    {
                        byte b = key[num];
                        key[num] = key[num2];
                        key[num2] = b;

                        byte b2 = input[i];
                        byte b3 = key[(key[num] + key[num2]) % key.Length];
                        array[i] = (byte)(b2 ^ b3);
                    }
                }
                result = array;
            }
            return result;
        }
    }
}
