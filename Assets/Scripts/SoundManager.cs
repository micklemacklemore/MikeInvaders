using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource musicSource;  // For background music
    private AudioSource sfxSource;    // For sound effects

    private Dictionary<string, AudioSource> activeSounds = new Dictionary<string, AudioSource>();

    [SerializeField] private AudioClip defaultMusic; // Assign default music in Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the manager across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // Make sure background music loops

        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    // 🔹 Play a music track (and stop any current music)
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    // 🔹 Play a sound effect (SFX)
    public void PlaySFX(AudioClip clip, string key = "", float volume = 1f, bool loop = false)
    {
        if (string.IsNullOrEmpty(key)) key = clip.name; // Use clip name if no key is provided

        if (activeSounds.ContainsKey(key))
        {
            StopSFX(key); // Stop the sound if it's already playing
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.loop = loop;
        newSource.Play();

        activeSounds[key] = newSource;

        if (!loop)
        {
            Destroy(newSource, clip.length); // Auto-destroy non-looping sounds
            activeSounds.Remove(key); // Remove reference
        }
    }

    // 🔹 Stop a specific sound effect
    public void StopSFX(string key)
    {
        if (activeSounds.ContainsKey(key))
        {
            Destroy(activeSounds[key]);
            activeSounds.Remove(key);
        }
    }

    // 🔹 Stop all sound effects
    public void StopAllSFX()
    {
        foreach (var source in activeSounds.Values)
        {
            Destroy(source);
        }
        activeSounds.Clear();
    }

    // 🔹 Stop music
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Play default music on start (Optional)
    void Start()
    {
        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }
}
