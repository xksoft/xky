using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Xky.Core.Common
{
    /// <summary>
    /// 文件相关操作类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        ///     获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattren"></param>
        /// <returns></returns>
        public static List<string> GetFileList(string path, string pattren)
        {
            return GetFileList(path, pattren, false);
        }

        /// <summary>
        ///     获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattren"></param>
        /// <param name="isHaveSubdir"></param>
        /// <returns></returns>
        public static List<string> GetFileList(string path, string pattren, bool isHaveSubdir)
        {
            var list = new List<string>();
            try
            {
                var s = Directory.GetFiles(Path.GetFullPath(path), pattren);
                list.AddRange(s);
                if (isHaveSubdir)
                {
                    var dirs = Directory.GetDirectories(path);
                    foreach (var t in dirs)
                        list.AddRange(GetFileList(t, pattren, true));
                }
            }
            catch
            {
                // ignored
            }

            return list;
        }
       

        /// <summary>
        ///     获取文件哈希值
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetFileHash(string filepath)
        {
            string hashcode;
            try
            {
                var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var md5 = new MD5CryptoServiceProvider();
                var byt = md5.ComputeHash(fs);
                hashcode = BitConverter.ToString(byt);
                hashcode = hashcode.Replace("-", "");
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                return "";
            }

            return hashcode;
        }
    }
}