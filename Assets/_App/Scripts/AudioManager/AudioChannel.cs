using DG.Tweening;
using UnityEngine;

public class AudioChannel<T>
{
    /// <summary>
    /// When ever play a music from this channel, this id will be set
    /// </summary>
    public T AudioKey;
    public bool IsPlay;
    public AudioSource Source;
    
    private Tween _autoStopTween;

    public bool IsMute
    {
        get => Source.mute;
        set => Source.mute = value;
    }

    public AudioChannel(AudioSource source)
    {
        Source = source;
    }

    public void Play(T audioKey, AudioClip clip, float volume, bool isMute, bool isLoop = true, bool isSetTimeStop = false, float stopAfter = 0,
        float delay = 0)
    {
        IsPlay = true;
        AudioKey = audioKey;
        Source.clip = clip;
        Source.loop = isLoop;
        Source.mute = isMute;
        Source.volume = volume;
        Source.PlayDelayed(delay);

        if (isSetTimeStop)
        {
            _autoStopTween = DOVirtual.DelayedCall(delay + stopAfter, Stop);
        }
        else if (!isLoop)
        {
            _autoStopTween = DOVirtual.DelayedCall(clip.length, Stop);
        }
    }

    public void Stop()
    {
        IsPlay = false;
        Source.Stop();
        _autoStopTween?.Kill();
    }
}