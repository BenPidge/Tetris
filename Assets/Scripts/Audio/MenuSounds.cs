using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuSounds : MonoBehaviour
{
    [SerializeField] private SoundEffect buttonClick;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayBtnClick()
    {
        buttonClick.PlayEffect(_audioSource);
    }
}