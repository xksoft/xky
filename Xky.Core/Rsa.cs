using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace Xky.Core
{
    public class Rsa
    {
        public static string DecrypteRsa(string s)
        {
            return DecryptByPublicKey(s,
                @"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAJcnpxfn++fmCG4jvC7mloxo92EUI+uG+X7mneyYVwIA1xDXIDjvHYy0l2UaKhpoWCiTnA2OmFRfnjkPfKAkWhcCAwEAAQ==");
        }

        private static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            var publicInfoByte = Convert.FromBase64String(s);
            Asn1Object.FromByteArray(publicInfoByte);
            var pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }

        private static AsymmetricKeyParameter GetPrivateKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            var privateInfoByte = Convert.FromBase64String(s);
            var priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            return priKey;
        }

        public static string EncryptByPrivateKey(string s, string key)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

            try
            {
                engine.Init(true, GetPrivateKeyParameter(key));
                var byteData = Encoding.UTF8.GetBytes(s);
                var resultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return Convert.ToBase64String(resultData);
                //Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string DecryptByPublicKey(string s, string key)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            try
            {
                engine.Init(false, GetPublicKeyParameter(key));

                var byteData = Convert.FromBase64String(s);

                if (byteData.Length % 64 > 0) throw new Exception("待解密的数据格式不正确");

                var array = new List<byte>();

                var buffersCount = byteData.Length / 64;

                for (var i = 0; i < buffersCount; i++) array.AddRange(engine.ProcessBlock(byteData, i * 64, 64));


                return Encoding.UTF8.GetString(array.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}