using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xky.Core.Common
{
    public sealed class StrHelper
    {
        #region SHA1加密

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static string Sha1(string str, bool isUpper)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var bytes_sha1_in = Encoding.Default.GetBytes(str);
            var bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            var str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            //str_sha1_out = str_sha1_out.Replace("-", "");
            return isUpper ? str_sha1_out.ToUpper() : str_sha1_out.ToLower();
        }

        #endregion

        #region 正则提取

        /// <summary>
        ///     正则提取
        /// </summary>
        /// <param name="html">源码</param>
        /// <param name="regex">正则表达式</param>
        /// <returns>结果</returns>
        public static List<string> GetRegex(string html, string regex)
        {
            var result = new List<string>();
            try
            {
                var mccc = Regex.Matches(html, regex, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                for (var i = 0; i < mccc.Count; i++)
                {
                    result.Add(mccc[i].Groups[0].Value);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Add("正则表达式有误：" + ex.Message);
                return result;
            }
        }

        #endregion

        #region 拼音转换

        /// <summary>
        ///     把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string ConvertToPy(string hzString)
        {
            // 匹配中文字符
            var regex = new Regex("^[\u4e00-\u9fa5]$");
            var pyString = "";
            var noWChar = hzString.ToCharArray();

            foreach (var t in noWChar)
            {
                // 中文字符
                if (regex.IsMatch(t.ToString()))
                {
                    var array = Encoding.Default.GetBytes(t.ToString());
                    int i1 = array[0];
                    int i2 = array[1];
                    var chrAsc = i1 * 256 + i2 - 65536;
                    if (chrAsc > 0 && chrAsc < 160)
                    {
                        pyString += t;
                    }
                    else
                    {
                        // 修正部分文字
                        if (chrAsc == -9254) // 修正“圳”字
                            pyString += "Zhen";
                        else
                        {
                            for (var i = (PyValue.Length - 1); i >= 0; i--)
                            {
                                if (PyValue[i] <= chrAsc)
                                {
                                    pyString += PyName[i];
                                    break;
                                }
                            }
                        }
                    }
                }
                // 非中文字符
                else
                {
                    pyString += t.ToString();
                }
            }
            return pyString;
        }

        /// <summary>
        ///     把汉字转换成拼音(首字母)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string GetCharSpellCode(string str, bool toUpper)
        {
            if (toUpper)
                return GetCharSpellCode(str).ToUpper();
            return GetCharSpellCode(str).ToLower();
        }

        private static string GetCharSpellCode(string cnChar)
        {
            var zw = Encoding.Default.GetBytes(cnChar);


            //如果是字母，则直接返回 

            if (zw.Length == 1)
            {
                return cnChar.ToUpper();
            }

            //   get   the     array   of   byte   from   the   single   char    

            int i1 = zw[0];

            int i2 = zw[1];

            long iCnChar = i1 * 256 + i2;

            #region table   of   the   constant   list

            //expresstion 

            //table   of   the   constant   list 

            // 'A';           //45217..45252 

            // 'B';           //45253..45760 

            // 'C';           //45761..46317 

            // 'D';           //46318..46825 

            // 'E';           //46826..47009 

            // 'F';           //47010..47296 

            // 'G';           //47297..47613 


            // 'H';           //47614..48118 

            // 'J';           //48119..49061 

            // 'K';           //49062..49323 

            // 'L';           //49324..49895 

            // 'M';           //49896..50370 

            // 'N';           //50371..50613 

            // 'O';           //50614..50621 

            // 'P';           //50622..50905 

            // 'Q';           //50906..51386 


            // 'R';           //51387..51445 

            // 'S';           //51446..52217 

            // 'T';           //52218..52697 

            //没有U,V 

            // 'W';           //52698..52979 

            // 'X';           //52980..53640 

            // 'Y';           //53689..54480 

            // 'Z';           //54481..55289 

            #endregion

            //   iCnChar match     the   constant 

            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }

            if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }

            if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }

            if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }

            if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }

            if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }

            if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }

            if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }

            if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }

            if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }

            if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }

            if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }


            if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }

            if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }

            if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }

            if ((iCnChar >= 50906) && (iCnChar <= .51386))
            {
                return "Q";
            }

            if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }

            if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }

            if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }

            if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }

            if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }

            if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }

            if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }

            return ("?");
        }

        #endregion

        #region 字符处理类

        /// <summary>
        ///     字符串截断
        /// </summary>
        /// <param name="str">原始字符</param>
        /// <param name="len">最长长度</param>
        /// <param name="morestr">超出部分表示符</param>
        /// <returns></returns>
        public static string SubString(string str, int len, string morestr, bool iscenter = false)
        {
            if (str.Length > len)
            {
                if (iscenter)
                {
                    return str.Substring(0, len / 2) + morestr + str.Substring(str.Length - len / 2, len / 2);
                }
                else
                {
                    return str.Substring(0, len) + morestr;
                }
            }
            return str;
        }

        #region url编码

        private static bool IsUrlSafeChar(char ch)
        {
            if ((((ch < 'a') || (ch > 'z')) && ((ch < 'A') || (ch > 'Z'))) && ((ch < '0') || (ch > '9')))
            {
                switch (ch)
                {
                    case '(':
                    case ')':
                    case '*':
                    case '-':
                    case '.':
                    case '!':
                        break;

                    case '+':
                    case ',':
                        goto Label_0051;

                    default:
                        if (ch != '_')
                        {
                            goto Label_0051;
                        }
                        break;
                }
            }
            return true;
        Label_0051:
            return false;
        }

        public static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        public static int HexToInt(char h)
        {
            if ((h >= '0') && (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }

        private static bool ValidateUrlEncodingParameters(ICollection<byte> bytes, int offset, int count)
        {
            if ((bytes == null) && (count == 0))
            {
                return false;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Count))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Count))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return true;
        }

        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            var data = e.GetBytes(str);
            return Encoding.ASCII.GetString(UrlByteEncode(data, 0, data.Length));
        }

        private static byte[] UrlByteEncode(byte[] bytes, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(bytes, offset, count))
            {
                return null;
            }
            var num = 0;
            var num2 = 0;
            for (var i = 0; i < count; i++)
            {
                var ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    num2++;
                }
            }
            if ((num == 0) && (num2 == 0))
            {
                if ((offset == 0) && (bytes.Length == count))
                {
                    return bytes;
                }
                var dst = new byte[count];
                Buffer.BlockCopy(bytes, offset, dst, 0, count);
                return dst;
            }
            var buffer = new byte[count + (num2 * 2)];
            var num3 = 0;
            for (var j = 0; j < count; j++)
            {
                var num6 = bytes[offset + j];
                var ch2 = (char)num6;
                if (IsUrlSafeChar(ch2))
                {
                    buffer[num3++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num3++] = 0x2b;
                }
                else
                {
                    buffer[num3++] = 0x25;
                    buffer[num3++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num3++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        #endregion

        /// <summary>
        ///     从二进制中获取到字符
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string GetStrByByte(byte[] data, ref Encoding encode)
        {
            if (encode == null)
                throw new ArgumentNullException("encode");
            var code1 = Encoding.Default;
            var code2 = Encoding.UTF8;
            var result1 = code1.GetString(data);
            var result2 = code2.GetString(data);

            var m1 = UrlEncode(result1, code1).Length;
            var m11 = UrlEncode(result1, code2).Length;
            var m2 = UrlEncode(result2, code1).Length;
            var m21 = UrlEncode(result2, code2).Length;
            if (Math.Abs(m2 - m21) <= Math.Abs(m1 - m11))
            {
                encode = code2;
                return result2;
            }
            encode = code1;
            return result1;
        }

        /// <summary>
        ///     从二进制中获取到字符
        /// </summary>
        public static string GetStr(byte[] data)
        {
            try
            {
                var e = Encoding.Default;
                return GetStrByByte(data, ref e);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        ///     从二进制中获取到字符
        /// </summary>
        public static string GetStr(byte[] data, ref Encoding encoding)
        {
            try
            {
                return GetStrByByte(data, ref encoding);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public static string GetMainDomain(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            if (!url.StartsWith("http"))
                url = "http://" + url + "/";
            var uri = new Uri(url);
            var domain = uri.Host.ToLower();


            var find = (from d in dn
                        where domain.EndsWith(d)
                        select d).FirstOrDefault();
            var result = "";
            var strs = domain.Split('.');
            if (find != null)
            {
                var l = find.Split('.').Length;
                for (var i = strs.Length - l; i < strs.Length; i++)
                {
                    if (i != strs.Length - 1)
                        result += strs[i] + ".";
                    else
                    {
                        result += strs[i];
                    }
                }
            }
            else
            {
                if (strs.Length <= 2)
                    return domain;
                else
                {
                    const int l = 1;
                    for (var i = strs.Length - l - 1; i < strs.Length; i++)
                    {
                        if (i != strs.Length - 1)
                            result += strs[i] + ".";
                        else
                        {
                            result += strs[i];
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region MD5加密

        /// <summary>
        ///     md5加密
        /// </summary>
        /// <param name="convertString">待加密的字符串</param>
        /// <returns>返回32位大写md5字符串</returns>
        public static string Md5(string convertString)
        {
            MD5 m = new MD5CryptoServiceProvider();
            var s = m.ComputeHash(Encoding.UTF8.GetBytes(convertString));
            return BitConverter.ToString(s).Replace("-", "");
        }

        /// <summary>
        ///     md5加密
        /// </summary>
        /// <param name="convertString">待加密的字符串</param>
        /// <param name="isUpper">返回的md5值是否是大写</param>
        /// <returns>返回32位md5字符串</returns>
        public static string Md5(string convertString, bool isUpper)
        {
            if (isUpper)
                return Md5(convertString);
            return Md5(convertString).ToLower();
        }

        /// <summary>
        ///     md5加密
        /// </summary>
        /// <param name="convertString">待加密的字符串</param>
        /// <param name="isUpper">返回的md5值是否是大写</param>
        /// <param name="is16">是否是16位md5</param>
        /// <returns>返回md5字符串</returns>
        public static string GetMd5(string convertString, bool isUpper, bool is16)
        {
            if (is16)
            {
                string t2;
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(convertString)), 4, 8);
                }
                t2 = t2.Replace("-", "");
                return isUpper ? t2.ToUpper() : t2.ToLower();
            }
            return isUpper ? Md5(convertString) : Md5(convertString).ToLower();
        }

        #endregion

        #region DES加密解密

        /// <summary>
        ///     DES解密方法
        /// </summary>
        /// <param name="pToDecrypt">待解密的密文</param>
        /// <param name="mymy">密钥（8位）</param>
        /// <returns>字符串</returns>
        public static string DESDecrypt(string pToDecrypt, string mymy)
        {
            if (pToDecrypt == "")
                pToDecrypt = "123";
            try
            {
                var inputByteArray = Convert.FromBase64String(pToDecrypt);
                using (var des = new DESCryptoServiceProvider())
                {
                    des.Key = Encoding.ASCII.GetBytes(mymy);
                    des.IV = Encoding.ASCII.GetBytes(mymy);
                    var ms = new MemoryStream();

                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                        cs.Dispose();
                    }


                    var str = ByteToString(ms.ToArray());
                    ms.Close();
                    ms.Dispose();
                    return str;
                }
            }
            catch (Exception)
            {
                //Tools.Log.log("错误：DES解密出错，原因:" + ex.ToString() + ex.StackTrace.ToString());
                return "";
            }
        }

        /// <summary>
        ///     DES加密方法
        /// </summary>
        /// <param name="pToEncrypt">待加密的字符串</param>
        /// <param name="mymy">密钥（8位）</param>
        /// <returns>字符串</returns>
        public static string DESEncrypt(string pToEncrypt, string mymy)
        {
            try
            {
                using (var des = new DESCryptoServiceProvider())
                {
                    var inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                    des.Key = Encoding.ASCII.GetBytes(mymy);
                    des.IV = Encoding.ASCII.GetBytes(mymy);
                    var ms = new MemoryStream();
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                        cs.Dispose();
                    }
                    var str = Convert.ToBase64String(ms.ToArray());
                    ms.Close();
                    ms.Dispose();
                    return str;
                }
            }
            catch (Exception)
            {
                //Tools.Log.log("错误：DES解密出错，原因:" + ex.ToString() + ex.StackTrace.ToString());
                return "";
            }
        }


        /// <summary>
        ///     des解密
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <returns></returns>
        public static byte[] DESDecrypt(byte[] inputByteArray)
        {
            for (var i = 0; i < inputByteArray.Length; i++)
            {
                inputByteArray[i] = (byte)(inputByteArray[i] + 11);
            }
            return inputByteArray;
        }

        /// <summary>
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <returns></returns>
        public static byte[] DESEncrypt(byte[] inputByteArray)
        {
            for (var i = 0; i < inputByteArray.Length; i++)
            {
                inputByteArray[i] = (byte)(inputByteArray[i] - 11);
            }
            return inputByteArray;
        }

        #endregion

        #region 拼音数组定义

        private static readonly string[] dn =
        {
            ".net.cn", ".com.cn", ".net.cn", ".org.cn", ".gov.cn", ".edu.cn", ".gz.cn", ".ha.cn", ".jl.cn", ".sh.cn",
            ".qh.cn", ".gx.cn", ".ah.cn", ".sx.cn", ".hk.cn", ".fj.cn", ".xz.cn", ".hb.cn", ".hl.cn", ".tj.cn",
            ".nx.cn", ".hi.cn", ".jx.cn", ".nm.cn", ".mo.cn", ".ac.cn", ".sn.cn", ".hn.cn", ".js.cn", ".cq.cn",
            ".xj.cn", ".sc.cn", ".sd.cn", ".ln.cn", ".yn.cn", ".bj.cn", ".gs.cn", ".gd.cn", ".zj.cn", ".he.cn",
            ".tw.cn", ".com", ".net", ".org", ".int", ".edu", ".gov", ".mil", ".biz", ".info", ".name", ".pro",
            ".museum", ".aero", ".coop", ".travel", ".asia", ".jobs", ".mobi", ".tel", ".cat", ".ac", ".cu", ".ie",
            ".mu", ".sm", ".ad", ".cv", ".il", ".mv", ".sn", ".ae", ".cx", ".im", ".mw", ".so", ".af", ".cy", ".in",
            ".mx", ".sr", ".ag", ".cz", ".io", ".my", ".st", ".ai", ".de", ".iq", ".mz", ".su", ".al", ".dj", ".ir",
            ".na", ".sv", ".am", ".dk", ".is", ".nc", ".sy", ".an", ".dm", ".it", ".ne", ".sz", ".ao", ".do", ".je",
            ".nf", ".tc", ".aq", ".dz", ".jm", ".ng", ".td", ".as", ".ec", ".jo", ".ni", ".tf", ".at", ".ee", ".jp",
            ".nl", ".tg", ".au", ".eg", ".ke", ".no", ".th", ".aw", ".eh", ".kg", ".np", ".tj", ".az", ".er", ".kh",
            ".nr", ".tk", ".ba", ".es", ".ki", ".nt", ".tm", ".bb", ".et", ".km", ".nu", ".tn", ".bd", ".fi", ".kn",
            ".nz", ".to", ".be", ".fj", ".kp", ".om", ".tp", ".bf", ".fk", ".kr", ".pa", ".tr", ".bg", ".fm", ".kw",
            ".pe", ".tt", ".bh", ".fr", ".ky", ".pf", ".tv", ".bi", ".fx", ".kz", ".pg", ".tw", ".bj", ".ga", ".la",
            ".ph", ".tz", ".bm", ".gb", ".lb", ".pk", ".ua", ".bo", ".ge", ".li", ".pl", ".ug", ".br", ".gf", ".lk",
            ".pm", ".uk", ".bs", ".gg", ".lr", ".pn", ".um", ".bt", ".gh", ".ls", ".pr", ".us", ".bv", ".gi", ".lt",
            ".pt", ".uy", ".bw", ".gl", ".lu", ".pw", ".uz", ".by", ".gm", ".lv", ".py", ".va", ".bz", ".gn", ".ly",
            ".qa", ".vc", ".ca", ".gp", ".ma", ".re", ".ve", ".cc", ".gq", ".mc", ".ro", ".vg", ".cd", ".gr", ".md",
            ".ru", ".vi", ".cf", ".gs", ".mg", ".rw", ".vn", ".cg", ".gt", ".mh", ".sa", ".vu", ".ch", ".gu", ".mk",
            ".sb", ".wf", ".ci", ".gw", ".ml", ".sd", ".ws", ".ck", ".gy", ".mm", ".se", ".ye", ".cl", ".hk", ".mn",
            ".sg", ".yt", ".cm", ".hn", ".mo", ".sh", ".yu", ".cn", ".hr", ".mp", ".si", ".zm", ".co", ".ht", ".mq",
            ".sj", ".zr", ".cr", ".hu", ".ms", ".sk", ".cs", ".id", ".mt", ".sl"
        };

        private static readonly int[] PyValue =
        {
            -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242, -20230, -20051, -20036,
            -20032, -20026, -20002, -19990, -19986, -19982, -19976, -19805, -19784, -19775, -19774, -19763,
            -19756, -19751, -19746, -19741, -19739, -19728, -19725, -19715, -19540, -19531, -19525, -19515,
            -19500, -19484, -19479, -19467, -19289, -19288, -19281, -19275, -19270, -19263, -19261, -19249,
            -19243, -19242, -19238, -19235, -19227, -19224, -19218, -19212, -19038, -19023, -19018, -19006,
            -19003, -18996, -18977, -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735,
            -18731, -18722, -18710, -18697, -18696, -18526, -18518, -18501, -18490, -18478, -18463, -18448,
            -18447, -18446, -18239, -18237, -18231, -18220, -18211, -18201, -18184, -18183, -18181, -18012,
            -17997, -17988, -17970, -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759, -17752,
            -17733, -17730, -17721, -17703, -17701, -17697, -17692, -17683, -17676, -17496, -17487, -17482,
            -17468, -17454, -17433, -17427, -17417, -17202, -17185, -16983, -16970, -16942, -16915, -16733,
            -16708, -16706, -16689, -16664, -16657, -16647, -16474, -16470, -16465, -16459, -16452, -16448,
            -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401, -16393, -16220, -16216,
            -16212, -16205, -16202, -16187, -16180, -16171, -16169, -16158, -16155, -15959, -15958, -15944,
            -15933, -15920, -15915, -15903, -15889, -15878, -15707, -15701, -15681, -15667, -15661, -15659,
            -15652, -15640, -15631, -15625, -15454, -15448, -15436, -15435, -15419, -15416, -15408, -15394,
            -15385, -15377, -15375, -15369, -15363, -15362, -15183, -15180, -15165, -15158, -15153, -15150,
            -15149, -15144, -15143, -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109,
            -14941, -14937, -14933, -14930, -14929, -14928, -14926, -14922, -14921, -14914, -14908, -14902,
            -14894, -14889, -14882, -14873, -14871, -14857, -14678, -14674, -14670, -14668, -14663, -14654,
            -14645, -14630, -14594, -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353, -14345,
            -14170, -14159, -14151, -14149, -14145, -14140, -14137, -14135, -14125, -14123, -14122, -14112,
            -14109, -14099, -14097, -14094, -14092, -14090, -14087, -14083, -13917, -13914, -13910, -13907,
            -13906, -13905, -13896, -13894, -13878, -13870, -13859, -13847, -13831, -13658, -13611, -13601,
            -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367, -13359, -13356, -13343,
            -13340, -13329, -13326, -13318, -13147, -13138, -13120, -13107, -13096, -13095, -13091, -13076,
            -13068, -13063, -13060, -12888, -12875, -12871, -12860, -12858, -12852, -12849, -12838, -12831,
            -12829, -12812, -12802, -12607, -12597, -12594, -12585, -12556, -12359, -12346, -12320, -12300,
            -12120, -12099, -12089, -12074, -12067, -12058, -12039, -11867, -11861, -11847, -11831, -11798,
            -11781, -11604, -11589, -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067,
            -11055, -11052, -11045, -11041, -11038, -11024, -11020, -11019, -11018, -11014, -10838, -10832,
            -10815, -10800, -10790, -10780, -10764, -10587, -10544, -10533, -10519, -10331, -10329, -10328,
            -10322, -10315, -10309, -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256, -10254
        };

        private static readonly string[] PyName =
        {
            "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei", "Ben",
            "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu", "Ba", "Cai", "Can",
            "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan", "Chang", "Chao", "Che", "Chen", "Cheng",
            "Chi", "Chong", "Chou", "Chu", "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong",
            "Cou", "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De",
            "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du", "Duan", "Dui",
            "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei", "Fen", "Feng", "Fo",
            "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge", "Gei", "Gen", "Geng", "Gong",
            "Gou", "Gu", "Gua", "Guai", "Guan", "Guang", "Gui", "Gun", "Guo", "Ha", "Hai", "Han",
            "Hang", "Hao", "He", "Hei", "Hen", "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan",
            "Huang", "Hui", "Hun", "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing",
            "Jiong", "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",
            "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui", "Kun", "Kuo",
            "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li", "Lia", "Lian", "Liang",
            "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou", "Lu", "Lv", "Luan", "Lue", "Lun",
            "Luo", "Ma", "Mai", "Man", "Mang", "Mao", "Me", "Mei", "Men", "Meng", "Mi", "Mian",
            "Miao", "Mie", "Min", "Ming", "Miu", "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang",
            "Nao", "Ne", "Nei", "Nen", "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning",
            "Niu", "Nong", "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan",
            "Pang", "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping", "Po",
            "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing", "Qiong", "Qiu", "Qu",
            "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re", "Ren", "Reng", "Ri", "Rong", "Rou",
            "Ru", "Ruan", "Rui", "Run", "Ruo", "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen",
            "Seng", "Sha", "Shai", "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu",
            "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song", "Sou", "Su", "Suan",
            "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang", "Tao", "Te", "Teng", "Ti", "Tian",
            "Tiao", "Tie", "Ting", "Tong", "Tou", "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai",
            "Wan", "Wang", "Wei", "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao",
            "Xie", "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan", "Yang",
            "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu", "Yuan", "Yue", "Yun",
            "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei", "Zen", "Zeng", "Zha", "Zhai", "Zhan",
            "Zhang", "Zhao", "Zhe", "Zhen", "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan",
            "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan", "Zui", "Zun", "Zuo"
        };

        #endregion

        #region 字符串转换

        public static string ByteToString(byte[] data, Encoding encoding)
        {
            var sr = new StreamReader(new MemoryStream(data), encoding);

            return sr.ReadToEnd();
        }

        public static string ByteToString(byte[] data)
        {
            return ByteToString(data, Encoding.UTF8);
        }

        #endregion
    }
}

