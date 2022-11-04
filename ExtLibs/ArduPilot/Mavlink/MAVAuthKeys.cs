using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Mavlink
{
    public class MAVAuthKeys
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string keyfile = Settings.GetUserDataDirectory() + "authkeys.xml";

        private static readonly Crypto Rij = new Crypto();

        public static AuthKeys Keys = new AuthKeys();

        static MAVAuthKeys()
        {
            Load();
        }

        public static void AddKey(string name, string seed)
        {
            // sha the user input string
            using (var signit = new SHA256CryptoServiceProvider())
            {
                var shauser = signit.ComputeHash(Encoding.UTF8.GetBytes(seed));
                Array.Resize(ref shauser, 32);

                Keys[name] = new AuthKey { Key = shauser, Name = name };
            }
        }

        public static void Save()
        {
            // save config
            var writer =
                new DataContractSerializer(typeof(AuthKeys),
                    new[] { typeof(AuthKey) });

            using (var fs = new FileStream(keyfile, FileMode.Create))
            using (var sw = new CryptoStream(fs, Rij.algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                writer.WriteObject(sw, Keys);
            }
        }

        internal static void Load()
        {
            if (!File.Exists(keyfile))
                return;

            try
            {
                var reader =
                    new DataContractSerializer(typeof(AuthKeys),
                        new[] { typeof(AuthKey) });

                using (var fs = new FileStream(keyfile, FileMode.Open))
                using (var sr = new CryptoStream(fs, Rij.algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    Keys = (AuthKeys)reader.ReadObject(sr);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        //https://msdn.microsoft.com/en-us/library/aa347850(v=vs.110).aspx

        [CollectionDataContract(ItemName = "AuthKeys", Namespace = "")]
        public class AuthKeys : Dictionary<string, AuthKey>
        {
        }

        [DataContract(Name = "AuthKey", Namespace = "")]
        public struct AuthKey
        {
            [DataMember] public string Name;
            [DataMember] public byte[] Key;
        }
    }
}