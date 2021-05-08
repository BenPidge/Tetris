using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TetrominoSounds : MonoBehaviour
{
    [SerializeField] private SoundEffect transformEffect;
    [SerializeField] private SoundEffect destroyEffect;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        TetrominoManager.TetrominoTransformed += PlayTransform;
        TetrominoManager.RowCleared += PlayRowDestruction;
    }

    private void OnDisable()
    {
        TetrominoManager.TetrominoTransformed -= PlayTransform;
        TetrominoManager.RowCleared -= PlayRowDestruction;
    }


    private void PlayTransform()
    {
        transformEffect.PlayEffect(_audioSource);
    }

    private void PlayRowDestruction(int input)
    {
        destroyEffect.PlayEffect(_audioSource);
    }
}
