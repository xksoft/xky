using System;
using System.Collections.Generic;
using System.Text;

namespace XSocket.Engine.Modules
{
    /// <remarks>
    ///     Provides methods for parsing a query string into an object, and vice versa.
    ///     Ported from the JavaScript module.
    ///     <see href="https://www.npmjs.org/package/parseqs">https://www.npmjs.org/package/parseqs</see>
    /// </remarks>
    public class ParseQS
    {
        /// <summary>
        ///     Compiles a querystring
        ///     Returns string representation of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Encode(Dictionary<string, string> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var sb = new StringBuilder();
            foreach (var key in obj.Keys)
            {
                if (sb.Length > 0) sb.Append("&");
                sb.Append(Global.EncodeURIComponent(key));
                sb.Append("=");
                sb.Append(Global.EncodeURIComponent(obj[key]));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Parses a simple querystring into an object
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Decode(string qs)
        {
            var qry = new Dictionary<string, string>();
            var pairs = qs.Split('&');
            for (var i = 0; i < pairs.Length; i++)
            {
                var pair = pairs[i].Split('=');
                //fix: query params used to contain leading questionmark, which resulted in wrong uri
                qry.Add(Global.DecodeURIComponent(pair[0].TrimStart('?')), Global.DecodeURIComponent(pair[1]));
            }

            return qry;
        }
    }
}