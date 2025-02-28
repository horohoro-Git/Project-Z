using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class DataEncryptionEditor : EditorWindow
{

    string key;
    string dataName;
    string encryptName;

    [MenuItem("Tools/DataEncryptionEditor")]
    public static void ShowWindow()
    {
        GetWindow<DataEncryptionEditor>("Data Encryption");
    }

    private void OnGUI()
    {
        dataName = EditorGUILayout.TextField("Data Name", dataName);
        encryptName = EditorGUILayout.TextField("Encrypt Name", encryptName);
        key = EditorGUILayout.TextField("AES Key", key);
        string dirPath = Path.Combine(Application.persistentDataPath, "PlayData");
       
        if(GUILayout.Button("Data Encrypt") && dataName.Length > 0 && encryptName.Length > 0 && key.Length > 0)
        {
            //string path = Path.Combine(dirPath, dataName);

            Encryption(dirPath, key, dataName, encryptName);
        }
    }


    void Encryption(string path, string key, string name, string encryptName)
    {
        string p = Path.Combine(path, name);
        string data = File.ReadAllText(p);

        byte[] plainBytes;

        string returnData = "";
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32));
            aes.GenerateIV();

            plainBytes = Encoding.UTF8.GetBytes(data);

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
                byte[] encryptedBytes = ms.ToArray();

                byte[] ivAndCipherText = new byte[aes.IV.Length + encryptedBytes.Length];
                Array.Copy(aes.IV, 0, ivAndCipherText, 0, aes.IV.Length);
                Array.Copy(encryptedBytes, 0, ivAndCipherText, aes.IV.Length, encryptedBytes.Length);

                returnData = Convert.ToBase64String(ivAndCipherText);
            }
        }

        string dataPath = Path.Combine(path, encryptName);
        File.WriteAllText(dataPath, returnData);
    }

}
