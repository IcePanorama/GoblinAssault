using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------- Audio Source -------")]
    public AudioClip background;
    public AudioClip shop;
    public AudioClip rock;
    public AudioClip goblin;
    public AudioClip Playerthrow;
    public AudioClip hurt;

    private bool shopMusicPlaying = false;

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlayShopMusic()
    {
        if (!shopMusicPlaying)
        {
            shopMusicPlaying = true;

            musicSource.clip = shop;
            musicSource.Play();
        }
    }

    public void StopShopMusic()
    {
        shopMusicPlaying = false;
    }
}

