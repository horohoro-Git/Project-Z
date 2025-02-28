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
       
        if(GUILayout.Button("Data Encrypt") && dataName.Length > 0 && encryptName.Length > 0 && key.Length == 16)
        {
            //string path = Path.Combine(dirPath, dataName);

            Encryption(dirPath, key, dataName, encryptName);
        }
    }


    void Encryption(string path, string key, string name, string encryptName)
    {
        string p = Path.Combine(path, name);
        string data = File.ReadAllText(p);


        string returnData = "";
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(data);
                    }
                }

                returnData = Convert.ToBase64String(ms.ToArray());
            }
        }

        string dataPath = Path.Combine(path, encryptName);
        File.WriteAllText(dataPath, returnData);
    }

}
