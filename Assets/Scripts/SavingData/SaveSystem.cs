using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

public class SaveSystem : MonoBehaviour
{
    public static UserAccount CurrentAccount = null;
    public static List<UserAccount> Accounts = new List<UserAccount>();
    private static List<string> _accountNames = new List<string>();
    public static List<Sprite> Sprites = new List<Sprite>();


    public static void SetupSprites()
    {
        Object[] spriteObjects = Resources.LoadAll("Sprites/Squares", typeof(Sprite));
        for (int i = 0; i < spriteObjects.Length; i++)
        {
            Sprites.Add((Sprite)spriteObjects[i]);
        }
    }

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
            catch (SerializationException e)
            { 
                Debug.Log(e.Message);
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
                catch (SerializationException e)
                { 
                    Debug.Log(e.Message);
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

    public static Sprite GetSprite(string name)
    {
        Debug.Log(Sprites[0].name);
        Debug.Log(name);
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i].name == name)
            {
                return Sprites[i];
            }
        }

        return null;
    }
    
    private static string ToPath(string file)
    {
        return Path.Combine(Application.persistentDataPath, file + ".sav");
    }
}