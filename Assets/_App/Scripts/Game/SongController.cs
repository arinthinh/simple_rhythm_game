using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Range(-200, 200)] private int _latencyOffsetMs = 0;

    private double _dspStart;
    private int _sampleRate;

    //public double SongTimeMsDSP => (AudioSettings.dspTime - _dspStart) * 1000.0 + _latencyOffsetMs;
    public double SongTimeMsDSP => (double)_audioSource.timeSamples / _sampleRate * 1000.0 + _latencyOffsetMs;
    public double SongLengthMs => (double)_audioSource.clip.samples / _sampleRate * 1000.0;
    public bool IsCompleted => SongTimeMsDSP - SongLengthMs >= 0;

    public void LoadSong(AudioClip songInfoAudioClip, int chartOffsetMs)
    {
        _audioSource.clip = songInfoAudioClip;
        _sampleRate = _audioSource.clip.frequency;
        _latencyOffsetMs = chartOffsetMs;
    }

    public void StartSong()
    {
        _dspStart = AudioSettings.dspTime + 0.1;
        _audioSource.PlayScheduled(_dspStart);
    }

    public void Pause()
    {
    }

    public void Resume()
    {
    }

    public void StopSong()
    {
    }
}