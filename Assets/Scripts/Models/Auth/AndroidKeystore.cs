#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Text;
using UnityEngine;

namespace PrimalConquest.Auth
{
    // AES-256-GCM via Android Keystore — keys are hardware-backed and never leave the secure element.
    internal static class AndroidKeystore
    {
        const string KeyAlias = "primal_conquest_key";

        static AndroidJavaObject GetKeyGenerator()
        {
            using var spec = new AndroidJavaObject(
                "android.security.keystore.KeyGenParameterSpec$Builder",
                KeyAlias,
                3); // PURPOSE_ENCRYPT | PURPOSE_DECRYPT
            spec.Call<AndroidJavaObject>("setBlockModes", "GCM")
                .Call<AndroidJavaObject>("setEncryptionPaddings", "NoPadding")
                .Call<AndroidJavaObject>("setKeySize", 256);
            var finalSpec = spec.Call<AndroidJavaObject>("build");

            using var keyGenClass = new AndroidJavaClass("javax.crypto.KeyGenerator");
            var keyGen = keyGenClass.CallStatic<AndroidJavaObject>("getInstance", "AES", "AndroidKeyStore");
            keyGen.Call("init", finalSpec);
            return keyGen;
        }

        static AndroidJavaObject GetOrCreateKey()
        {
            using var ksClass = new AndroidJavaClass("java.security.KeyStore");
            var ks = ksClass.CallStatic<AndroidJavaObject>("getInstance", "AndroidKeyStore");
            ks.Call("load", null);

            if (!ks.Call<bool>("containsAlias", KeyAlias))
            {
                using var keyGen = GetKeyGenerator();
                keyGen.Call<AndroidJavaObject>("generateKey");
            }

            return ks.Call<AndroidJavaObject>("getKey", KeyAlias, null);
        }

        public static string Encrypt(string plaintext)
        {
            var key = GetOrCreateKey();

            using var cipherClass = new AndroidJavaClass("javax.crypto.Cipher");
            var cipher = cipherClass.CallStatic<AndroidJavaObject>("getInstance", "AES/GCM/NoPadding");
            cipher.Call("init", 1, key); // ENCRYPT_MODE = 1

            var iv  = cipher.Call<byte[]>("getIV");
            var raw = Encoding.UTF8.GetBytes(plaintext);
            var enc = cipher.Call<byte[]>("doFinal", raw);

            // Prepend 12-byte IV so we can recover it on decryption
            var result = new byte[iv.Length + enc.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(enc, 0, result, iv.Length, enc.Length);
            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string ciphertext)
        {
            var key  = GetOrCreateKey();
            var data = Convert.FromBase64String(ciphertext);

            var iv  = new byte[12];
            var enc = new byte[data.Length - 12];
            Buffer.BlockCopy(data, 0, iv, 0, 12);
            Buffer.BlockCopy(data, 12, enc, 0, enc.Length);

            using var specClass = new AndroidJavaClass("javax.crypto.spec.GCMParameterSpec");
            var spec = new AndroidJavaObject("javax.crypto.spec.GCMParameterSpec", 128, iv);

            using var cipherClass = new AndroidJavaClass("javax.crypto.Cipher");
            var cipher = cipherClass.CallStatic<AndroidJavaObject>("getInstance", "AES/GCM/NoPadding");
            cipher.Call("init", 2, key, spec); // DECRYPT_MODE = 2

            var dec = cipher.Call<byte[]>("doFinal", enc);
            return Encoding.UTF8.GetString(dec);
        }
    }
}
#endif
