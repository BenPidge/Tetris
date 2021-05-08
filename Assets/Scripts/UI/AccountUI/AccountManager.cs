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
    protected MenuSounds Sounds;
    
    public void Start()
    {
        SaveSystem.SetupSprites();
        ChangeSelected(SaveSystem.CurrentAccount);
        Sounds = FindObjectOfType<MenuSounds>();
    }
    
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
        Sounds.PlayBtnClick();
        string username = GetComponentInChildren<InputField>().text;
        username = SaveSystem.ValidName(username);
        UserAccount newAccount = new UserAccount(SaveSystem.ValidName(username));
        
        NewAccountAdded?.Invoke(newAccount);
        ChangeSelected(newAccount);
        SaveSystem.AddAccount(newAccount);
        SaveSystem.CurrentAccount = newAccount;
    }
    
    private void ChangeSelected(UserAccount account)
    {
        if (account == null) return;
        SaveSystem.SetCurrentAccount(account);
        currentAccountName.text = account.getUsername();
        currentHighscoreNum.text = account.getHighscore().ToString();
    }
}