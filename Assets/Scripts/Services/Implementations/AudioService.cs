public class AudioService : IAudioService
{
    public void PlayMusic(string trackName)
    {
        // Плейсхолдер — подставим позже
        UnityEngine.Debug.Log($"Воспроизводится музыка: {trackName}");
    }

    public void SetVolume(float volume)
    {
        UnityEngine.AudioListener.volume = volume;
    }
}