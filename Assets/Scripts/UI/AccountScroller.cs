using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountScroller : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private List<string> names = new List<string>();
    
    private void Start()
    {
        SaveSystem.LoadAccounts();
        Fill();
    }

    private void OnEnable()
    {
        AccountManager.NewAccountAdded += AddAccountSelection;
    }

    private void OnDisable()
    {
        AccountManager.NewAccountAdded -= AddAccountSelection;
    }
    
    
    
    private void Fill()
    {
        List<UserAccount> accounts = SaveSystem.Accounts;
        
        for (int i = 0; i < accounts.Count; i++)
        {
            AddAccountSelection(accounts[i]);
        }
    }

    private void AddAccountSelection(UserAccount newAccount)
    {
        if (names.Contains(newAccount.getUsername())) return;
        
        GameObject nextAcctSelection = Instantiate(prefab, transform);
        nextAcctSelection.GetComponent<AccountScrollerItem>().SetAccount(newAccount);
        names.Add(newAccount.getUsername());
    }
}
