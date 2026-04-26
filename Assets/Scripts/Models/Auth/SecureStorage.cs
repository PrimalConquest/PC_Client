using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PrimalConquest.Auth
{
    // Encrypted key-value store.
    // Android builds: AES-256-GCM via hardware-backed Android Keystore.
    // Editor / other platforms: AES-256-CBC with a device-derived key (weaker but functional for dev).
    public static class SecureStorage
    {
        public static void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                PlayerPrefs.DeleteKey(PrefKey(key));
                PlayerPrefs.Save();
                return;
            }

            string encrypted;
#if UNITY_ANDROID && !UNITY_EDITOR
            encrypted = AndroidKeystore.Encrypt(value);
#else
            encrypted = EditorEncrypt(value);
#endif
            PlayerPrefs.SetString(PrefKey(key), encrypted);
            PlayerPrefs.Save();
        }

        public static string Get(string key)
        {
            var stored = PlayerPrefs.GetString(PrefKey(key), "");
            if (string.IsNullOrEmpty(stored)) return "";

            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return AndroidKeystore.Decrypt(stored);
#else
                return EditorDecrypt(stored);
#endif
            }
            catch
            {
                // Corrupt or migrated — treat as missing
                PlayerPrefs.DeleteKey(PrefKey(key));
                return "";
            }
        }

        public static void Delete(string key)
        {
            PlayerPrefs.DeleteKey(PrefKey(key));
            PlayerPrefs.Save();
        }

        static string PrefKey(string key) => $"sec_{key}";

        // ── Editor fallback (AES-256-CBC) ─────────────────────────────────────

        static byte[] DeriveEditorKey()
        {
            // Device-unique identifier as KDF seed — not hardware-backed but avoids plaintext storage
            var seed = Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier + "primal_conquest_salt");
            using var sha = SHA256.Create();
            return sha.ComputeHash(seed);
        }

        static string EditorEncrypt(string plaintext)
        {
            var key = DeriveEditorKey();
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var enc = aes.CreateEncryptor();
            var raw = Encoding.UTF8.GetBytes(plaintext);
            var cipher = enc.TransformFinalBlock(raw, 0, raw.Length);
            var result = new byte[aes.IV.Length + cipher.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);
            return Convert.ToBase64String(result);
        }

        static string EditorDecrypt(string ciphertext)
        {
            var key  = DeriveEditorKey();
            var data = Convert.FromBase64String(ciphertext);
            using var aes = Aes.Create();
            aes.Key = key;
            var iv     = new byte[16];
            var cipher = new byte[data.Length - 16];
            Buffer.BlockCopy(data, 0, iv, 0, 16);
            Buffer.BlockCopy(data, 16, cipher, 0, cipher.Length);
            aes.IV = iv;
            using var dec = aes.CreateDecryptor();
            var plain = dec.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(plain);
        }
    }
}
