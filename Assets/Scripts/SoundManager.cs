using UnityEngine;
using System;
using UnityEngine.UI;

[System.Serializable] // Makes this class editable in the Inspector
public class Sound
{
    public string name;     // Identifier for the sound
    public AudioClip clip;  // The actual audio file
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Singleton instance for global access

    private AudioSource bgmSource; // AudioSource for background music
    private AudioSource sfxSource; // AudioSource for sound effects

    [SerializeField] private Sound bgmSound;     // Background music to play at start
    [SerializeField] private Sound[] sfxSounds;  // List of available sound effects

    private void Awake()
    {
        // Set up singleton instance
        if (instance == null)
        {
            instance = this;
        }

        // Get AudioSources from child objects (0 = BGM, 1 = SFX)
        bgmSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        sfxSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void Start()
    {
        bgmSource.loop = true; // Background music loops by default
        PlayBGM(bgmSound);     // Start playing the default BGM
    }

    // Plays background music
    public void PlayBGM(Sound bgm)
    {
        bgmSource.clip = bgm.clip;
        bgmSource.Play();
    }

    // Plays a sound effect by name
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        sfxSource.PlayOneShot(s.clip); // Plays without interrupting other SFX
    }

    // Sets volume for both BGM and SFX (clamped between 0 and 1)
    public void SetVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
