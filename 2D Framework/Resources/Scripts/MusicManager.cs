using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("🎶 Music Settings")]
    public AudioSource musicSource;
    public AudioClip loopMusic;

    [Range(0f, 1f)] public float defaultVolume = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitMusic()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = loopMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        musicSource.Play();
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ToggleMusic(bool on)
    {
        if (on) musicSource.Play();
        else musicSource.Pause();

        PlayerPrefs.SetInt("MusicEnabled", on ? 1 : 0);
    }

    public bool IsPlaying()
    {
        return musicSource.isPlaying;
    }
}
