using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static UserAccount CurrentAccount;
    public static List<UserAccount> Accounts = new List<UserAccount>();
    private static List<string> _accountNames = new List<string>();



    public static void SetCurrentAccount(UserAccount account)
    {
        if (!Accounts.Contains(account)) return;
        CurrentAccount = Accounts[Accounts.IndexOf(account)];
    }
    
    public static void SaveAccount(UserAccount data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(ToPath(data.getUsername()), FileMode.Create))
        {
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (SerializationException)
            {
            }
        }
    }

    public static string ValidName(string name)
    {
        name = name.Trim();

        if (_accountNames.Contains(name))
        {
            int suffixVal = 1;
            while (_accountNames.Contains(name + " (" + suffixVal + ")"))
            {
                suffixVal++;
            }

            name = name + " (" + suffixVal + ")";
        }

        return name;
    }
    
    public static void SaveGame(GameSave save)
    {
        CurrentAccount.Save = save;
        SaveAccount(CurrentAccount);
    }
    
    public static void LoadAccounts()
    {
        if (!Directory.Exists(Application.persistentDataPath)) return;
        
        BinaryFormatter formatter = new BinaryFormatter();
        string[] files = Directory.GetFiles(Application.persistentDataPath, @"*.sav");
        UserAccount newAccount;
        
        for (int i = 0; i < files.Length; i++)
        {
            using (FileStream stream = new FileStream(files[i], FileMode.Open))
            {
                try
                {
                    newAccount = formatter.Deserialize(stream) as UserAccount;
                    if (newAccount == null) throw new SerializationException();
                    AddAccount(newAccount);
                }
                catch (SerializationException)
                {
                }
            }
        }
    }

    public static void DeleteSave(string file)
    {
        try
        {
            File.Delete(ToPath(file));
        }
        catch (FileNotFoundException)
        {
        }
    }

    public static void AddAccount(UserAccount newAccount)
    {
        Accounts.Add(newAccount);
        _accountNames.Add(newAccount.getUsername());
    }
    
    private static string ToPath(string file)
    {
        return Path.Combine(Application.persistentDataPath, file + ".sav");
    }
}