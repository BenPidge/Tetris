
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] protected TransitionManager transition;
    protected MenuSounds Sounds;
    protected void Start()
    {
        Sounds = FindObjectOfType<MenuSounds>();
    }

    protected void BtnClicked(string nextMenu)
    {
        Sounds.PlayBtnClick();
        StartCoroutine(transition.LoadSceneWithAnim(nextMenu));
    }
}