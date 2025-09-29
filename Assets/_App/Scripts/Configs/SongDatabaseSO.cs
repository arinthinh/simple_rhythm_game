using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SongDatabaseSO", menuName = "ScriptableObject/SongDatabaseSO", order = 0)]
public class SongDatabaseSO : ScriptableObject
{
    public SongInfo[] Songs;
    
    public SongInfo GetSongById(string id)
    {
        foreach (var song in Songs)
        {
            if (song.SongName == id) return song;
        }
        return null;
    }
}

[Serializable]
public class SongInfo
{
    public string SongName;
    public AudioClip AudioClip;
    public string BeatmapJson;
    
    public MiniChart GetChart()
    {
        var chart = JsonUtility.FromJson<MiniChart>(BeatmapJson);
        return chart;
    }
}

[Serializable]
public class MiniChart
{
    public int Lanes;
    public int OffsetMs;
    public MiniNote[] Notes;
}

[Serializable]
public class MiniNote
{
    public int T;
    public int Lane;
}