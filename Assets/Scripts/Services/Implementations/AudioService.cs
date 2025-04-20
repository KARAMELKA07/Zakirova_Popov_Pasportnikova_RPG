using UnityEngine;

public class AudioService : IAudioService
{
    private AudioSource audioSource;

    public AudioService()
    {
        GameObject bootstrapper = GameObject.FindObjectOfType<GameBootstrapper>().gameObject;
        audioSource = bootstrapper.GetComponent<AudioSource>();
    }

    public void PlayMusic(string trackName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{trackName}");
        if (clip == null)
        {
            Debug.LogError($"Не удалось загрузить трек: {trackName}");
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
}