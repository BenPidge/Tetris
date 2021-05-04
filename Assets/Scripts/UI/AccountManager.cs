using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AccountManager : MonoBehaviour
{
    public static event Action<UserAccount> NewAccountAdded;
    [SerializeField] private TextMeshProUGUI currentAccountName;
    [SerializeField] private TextMeshProUGUI currentHighscoreNum;
    
    private void OnEnable()
    {
        AccountScrollerItem.AccountSelected += ChangeSelected;
    }

    private void OnDisable()
    {
        AccountScrollerItem.AccountSelected -= ChangeSelected;
    }



    public void AddAccount()
    {
        string username = GetComponentInChildren<InputField>().text;
        username = SaveSystem.ValidName(username);
        UserAccount newAccount = new UserAccount(SaveSystem.ValidName(username));
        
        NewAccountAdded?.Invoke(newAccount);
        ChangeSelected(newAccount);
        SaveSystem.AddAccount(newAccount);
    }
    
    private void ChangeSelected(UserAccount account)
    {
        SaveSystem.SetCurrentAccount(account);
        currentAccountName.text = account.getUsername();
        currentHighscoreNum.text = account.getHighscore().ToString();
    }
}