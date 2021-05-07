using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountScrollerItem : MonoBehaviour
{
    public static event Action<UserAccount> AccountSelected;

    [SerializeField] private Text btnText;
    [SerializeField] private TextMeshProUGUI highscore;
    [SerializeField] private TextMeshProUGUI hasSave;
    private UserAccount _account;

    public void SetAccount(UserAccount account)
    {
        _account = account;
        btnText.text = _account.getUsername();
        highscore.text = "Highscore: " + _account.getHighscore();
        hasSave.text = "Has Save: " + (_account.Save != null).ToString().ToLower();
    }

    public void Chosen()
    {
        AccountSelected?.Invoke(_account);
    }
}