using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string key;
        public AudioClip clip;
    }

    [Header("Sound Effects")]
    [SerializeField] private List<Sound> soundEffects = new List<Sound>();
    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool loopMusic = true;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private float musicVolume = 1f; 

    void Awake()
    {
        // Populate dictionary
        foreach (var sound in soundEffects)
        {
            if (!soundDictionary.ContainsKey(sound.key))
            {
                soundDictionary.Add(sound.key, sound.clip);
            }
        }

        // Play background music if assigned
        if (backgroundMusic)
        {
            PlayMusic(backgroundMusic, loopMusic, musicVolume);
        }
    }

    public void PlaySoundEffect(string key, float volume = 1f)
    {
        if (soundDictionary.TryGetValue(key, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"Sound key '{key}' not found in SoundManager.");
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = volume; 
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
