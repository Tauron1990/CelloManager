using System;
using System.IO;
using System.Security.Cryptography;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Logic
{
    [Export(typeof(IPasswordHasher))]
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string GetPassword(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash)) return string.Empty;

            byte[] bytes = Convert.FromBase64String(hash);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int keyCount = reader.ReadInt32();
                    byte[] key = reader.ReadBytes(keyCount);
                    int ivCount = reader.ReadInt32();
                    byte[] iv = reader.ReadBytes(ivCount);
                    int passwordCount = reader.ReadInt32();
                    byte[] password = reader.ReadBytes(passwordCount);

                    return Decrypt(password, key, iv);
                }
            }
        }

        public string HashPassword(string password)
        {
            var erg = Encrypt(password);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    writer.Write(erg.Key.Length);
                    writer.Write(erg.Key, 0, erg.Key.Length);

                    writer.Write(erg.Iv.Length);
                    writer.Write(erg.Iv, 0, erg.Key.Length);

                    writer.Write(erg.Password.Length);
                    writer.Write(erg.Password, 0, erg.Key.Length);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private (byte[] Password, byte[] Key, byte[] Iv) Encrypt(string password)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {

                byte[] iv = aesAlg.IV;
                byte[] key = aesAlg.Key;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, iv);

                byte[] encrypted;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(password);
                        encrypted = msEncrypt.ToArray();
                    }
                }

                return (encrypted, key, iv);
            }
        }

        private string Decrypt(byte[] password, byte[] key, byte[] iv)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(password))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}