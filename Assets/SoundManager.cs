using UnityEngine;
using System;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [SerializeField] private Sound bgmSound;
    [SerializeField] private Sound[] sfxSounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

         bgmSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
         sfxSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();

    }

    private void Start()
    {
        bgmSource.loop = true;

        PlayBGM(bgmSound);
        
    }

    public void PlayBGM(Sound bgm)
    {
        
        bgmSource.clip = bgm.clip;
        bgmSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        
        sfxSource.PlayOneShot(s.clip);
    }

    public void SetVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
        sfxSource.volume = Mathf.Clamp01(volume);

    }


    
}


