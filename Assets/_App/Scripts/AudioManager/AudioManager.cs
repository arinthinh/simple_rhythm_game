using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("MIXER")]
    [SerializeField] private AudioMixerGroup _musicAudioMixerGroup;
    [SerializeField] private AudioMixerGroup _soundAudioMixerGroup;

    [Header("AUDIO CLIPS")]
    [SerializeField] private List<AudioClip> _musicClipList;
    [SerializeField] private List<AudioClip> _soundClipList;

    [Header("CONFIGS")]
    [SerializeField] private int _maxMusicChannel = 1;
    [SerializeField] private int _maxSoundChannel = 50;

    private readonly List<AudioChannel<EMusic>> _musicChannels = new();
    private readonly List<AudioChannel<ESound>> _soundChannels = new();

    private Dictionary<EMusic, AudioClip> _musicClipDic = new();
    private Dictionary<ESound, AudioClip> _soundClipDic = new();

    private bool _isMuteMusic;
    private bool _isMuteSound;

    public bool IsMuteMusic
    {
        get => _isMuteMusic;
        set
        {
            if (value == _isMuteMusic) return;
            _isMuteMusic = value;
            foreach (var channel in _musicChannels)
            {
                channel.IsMute = _isMuteMusic;
            }
        }
    }

    public bool IsMuteSound
    {
        get => _isMuteSound;
        set
        {
            if (value == _isMuteSound) return;
            _isMuteSound = value;
            foreach (var channel in _soundChannels)
            {
                channel.IsMute = _isMuteSound;
            }
        }
    }
    
    public static AudioManager Instance { get; private set; }

    public void Awake()
    {
        _musicClipDic = _musicClipList.ToDictionary(clip => (EMusic)Enum.Parse(typeof(EMusic), clip.name), clip => clip);
        _soundClipDic = _soundClipList.ToDictionary(clip => (ESound)Enum.Parse(typeof(ESound), clip.name), clip => clip);
        Instance = this;
    }

    #region MUSIC CONTROLLERS

    public AudioChannel<EMusic> PlayMusic(EMusic musicKey, float volume)
    {
        var channel = GetFreeMusicChannel();
        if (channel == null) return null;

        var musicAudioClip = GetMusicAudioClip(musicKey);
        channel.Play(musicKey, musicAudioClip, volume, _isMuteMusic);

        return channel;
    }

    public void StopMusic(EMusic musicKey)
    {
        _musicChannels.FirstOrDefault(m => m.AudioKey == musicKey)?.Stop();
    }

    public AudioChannel<EMusic> GetFreeMusicChannel()
    {
        var result = _musicChannels.FirstOrDefault(c => !c.IsPlay);
        return result ?? CreateNewMusicChannel();
    }

    public AudioChannel<EMusic> CreateNewMusicChannel()
    {
        if (_musicChannels.Count >= _maxMusicChannel)
        {
            Debug.LogError("Maximum music channels reached. Please decrease the number of music channels.");
            return null;
        }
        var newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.outputAudioMixerGroup = _musicAudioMixerGroup;
        var newChannel = new AudioChannel<EMusic>(newAudioSource);
        _musicChannels.Add(newChannel);
        return newChannel;
    }

    public AudioClip GetMusicAudioClip(EMusic musicKey)
    {
        if (_musicClipDic.TryGetValue(musicKey, out var clip)) return clip;
        Debug.LogError($"Music clip '{musicKey}' is not found in the list of music clips.");
        return null;
    }

    #endregion // MUSIC CONTROLLERS

    #region SOUND CONTROLLERS

    public AudioChannel<ESound> PlaySound(ESound soundKey, float volume = 1, float delay = 0, bool isLoop = false, bool isSetTimeStop = false,
        float timeStop = 0)
    {
        var channel = GetFreeSoundChannel();
        if (channel == null) return null;

        var soundAudioClip = GetSoundAudioClip(soundKey);
        if (soundAudioClip == null) return null;

        channel.Play(soundKey, soundAudioClip, volume, _isMuteSound, isLoop, isSetTimeStop, timeStop, delay);
        return channel;
    }

    public AudioChannel<ESound> GetFreeSoundChannel()
    {
        var result = _soundChannels.FirstOrDefault(c => !c.IsPlay);
        return result ?? CreateNewSoundChannel();
    }

    public AudioChannel<ESound> CreateNewSoundChannel()
    {
        if (_soundChannels.Count >= _maxSoundChannel)
        {
            Debug.LogError("Maximum sound channels reached. Please decrease the number of sound channels.");
            return null;
        }
        var newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.outputAudioMixerGroup = _soundAudioMixerGroup;
        var newChannel = new AudioChannel<ESound>(newAudioSource);
        _soundChannels.Add(newChannel);
        return newChannel;
    }

    public bool IsSoundPlaying(ESound soundKey)
    {
        return _soundChannels.Any(c => c.AudioKey == soundKey && c.IsPlay);
    }

    public AudioClip GetSoundAudioClip(ESound soundKey)
    {
        if (_soundClipDic.TryGetValue(soundKey, out var clip)) return clip;
        Debug.LogError($"Sound clip '{soundKey}' is not found in the list of sound clips.");
        return null;
    }

    public void StopSound(ESound soundKey)
    {
        _soundChannels
            .Where(channel => channel.AudioKey == soundKey)
            .ToList()
            .ForEach(channel => channel.Stop());
    }

    public void StopAllSound()
    {
        _soundChannels.ForEach(channel => channel.Stop());
    }

    #endregion // SOUND CONTROLLERS


#if UNITY_EDITOR
    public List<AudioClip> SoundClips => _soundClipList;
    public List<AudioClip> MusicClips => _musicClipList;

#endif // EDITOR
}