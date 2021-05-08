using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect", menuName = "ScriptableObjects/Sound Effect", order = 1)]
public class SoundEffect : ScriptableObject
{
    [SerializeField] private AudioClip effectAudio;
    [SerializeField] private float volume;
    [SerializeField] private float pitch;

    public void PlayEffect(AudioSource source)
    {
        source.pitch = pitch;
        source.PlayOneShot(effectAudio, volume);
    }
}
