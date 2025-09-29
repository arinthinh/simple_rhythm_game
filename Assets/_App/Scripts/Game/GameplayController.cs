using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("CONFIGS")]
    [SerializeField] private SongDatabaseSO _songDatabase;
    [SerializeField] private float _laneHeight = 8f;
    [SerializeField] private float _spawnLeadTimeMs = 1500f;
    [SerializeField] private int _scorePerPerfect = 100;
    [SerializeField] private int _scorePerGood = 50;
    [SerializeField] private float _perfectMs = 60f;
    [SerializeField] private float _goodMs = 110f;

    [Header("CONTROLLERS")]
    [SerializeField] private NoteFactory _noteFactory;
    [SerializeField] private SongController _songController;

    [Header("LANES")]
    [SerializeField] private LayerMask _noteMask;

    private GameScoreData _currentScoreData;
    private GameplayScreen _gameplayScreen;

    private bool _isPlaying = false;
    private int _currentNoteIndex;
    private MiniChart _chart;

    private readonly List<RhythmNote> _currentNotes = new();

    private void Start()
    {
        EventBus.Subscribe<PlayButtonClickedEvent>(OnPlayButtonClicked);
    }

    public void StartGameplay()
    {
        InitData();
        InitUI();
        HandleSpawnNotes();
        _songController.StartSong();
        return;

        void InitData()
        {
            var randomSongIndex = UnityEngine.Random.Range(0, _songDatabase.Songs.Length);
            var songInfo = _songDatabase.Songs[randomSongIndex];
            _currentScoreData = new();
            _isPlaying = true;
            _chart = songInfo.GetChart();
            _currentNotes.Clear();
            _currentNoteIndex = 0;
            _songController.LoadSong(songInfo.AudioClip, _chart.OffsetMs);
        }

        void InitUI()
        {
            _gameplayScreen = UIManager.Instance.ShowScreen<GameplayScreen>();
            _gameplayScreen.UpdateScore(0);
            _gameplayScreen.UpdateProgress(1);
            _gameplayScreen.UpdateCombo(0);
        }
    }

    private void HandleSpawnNotes()
    {
        double currentSongMs = _songController.SongTimeMsDSP;
        while (_currentNoteIndex < _chart.Notes.Length)
        {
            var noteData = _chart.Notes[_currentNoteIndex];
            if (noteData.T - currentSongMs <= _spawnLeadTimeMs)
            {
                var note = _noteFactory.SpawnNote(noteData.Lane);
                note.Init(targetTimeMs: noteData.T, leadMs: _spawnLeadTimeMs, laneHeight: _laneHeight);
                _currentNotes.Add(note);
                _currentNoteIndex++;
            }
            else break;
        }
    }


    private void Update()
    {
        if (!_isPlaying) return;
        UpdateSongProgressBar();
        HandleSpawnNotes();
        HandleMoveNotes();
        HandleJudgeTapNote();
        HandleDespawnNotes();
        CheckForCompletion();
    }

    private void UpdateSongProgressBar()
    {
        var progress = 1 - _songController.SongTimeMsDSP / _songController.SongLengthMs;
        _gameplayScreen.UpdateProgress((float)progress);
    }

    private void HandleMoveNotes()
    {
        foreach (var note in _currentNotes)
        {
            note.Move(_songController.SongTimeMsDSP);
        }
    }

    private void HandleJudgeTapNote()
    {
        var songMs = _songController.SongTimeMsDSP;
        var note = GetTapNote();
        if (note == null) return; // No note tap
        var judgeType = GetJudgement(note.TargetTimeMs, songMs);
        var score = judgeType switch
        {
            JudgeType.Perfect => _scorePerPerfect,
            JudgeType.Good => _scorePerGood,
            _ => 0
        };
        if (score > 0) // Hit
        {
            _currentScoreData.Score += score;
            _gameplayScreen.UpdateScore(_currentScoreData.Score);
            _currentScoreData.CurrentCombo++;
            if (_currentScoreData.CurrentCombo > _currentScoreData.MaxCombo)
            {
                _currentScoreData.MaxCombo = _currentScoreData.CurrentCombo;
            }
        }
        else // Miss
        {
            _currentScoreData.Misses++;
            _currentScoreData.CurrentCombo = 0;
        }
        _gameplayScreen.ShowJudgeText(judgeType);
        _gameplayScreen.UpdateCombo(_currentScoreData.CurrentCombo);
        _currentNotes.Remove(note);
        note.PlayDisappearAnimation(() => _noteFactory.ReleaseNote(note));
    }

    /// <summary>
    /// Get a tap note if player taps on it
    /// </summary>
    private RhythmNote GetTapNote()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics2D.Raycast(ray.origin, ray.direction, 100f, _noteMask))
            {
                var hit2D = Physics2D.Raycast(ray.origin, ray.direction, 100f, _noteMask);
                var note = hit2D.collider?.GetComponent<RhythmNote>();
                return note;
            }
        }
        return null;
    }

    /// <summary>
    /// Despawn note if it goes out of screen and count as miss
    /// </summary>
    private void HandleDespawnNotes()
    {
        foreach (var note in _currentNotes.ToArray())
        {
            if (note.IsOutOfScreen)
            {
                _currentNotes.Remove(note);
                note.PlayDisappearAnimation(() => _noteFactory.ReleaseNote(note));
                _currentScoreData.Misses++;
                _currentScoreData.CurrentCombo = 0;
                _gameplayScreen.UpdateCombo(_currentScoreData.CurrentCombo);
                _gameplayScreen.ShowJudgeText(JudgeType.Miss);
            }
        }
    }

    private void CheckForCompletion()
    {
        if (_songController.IsCompleted && _currentNotes.Count == 0)
        {
            _isPlaying = false;
            UIManager.Instance.HideScreen<GameplayScreen>();
            UIManager.Instance.ShowScreen<WinScreen>()
                .SetData(_currentScoreData.Score, _currentScoreData.MaxCombo);
        }
    }

    private void OnPlayButtonClicked(PlayButtonClickedEvent eventData)
    {
        StartGameplay();
    }

    private JudgeType GetJudgement(double noteTargetTimeMs, double songTimeMsDSP)
    {
        double d = System.Math.Abs(noteTargetTimeMs - songTimeMsDSP);
        if (d <= _perfectMs)
        {
            return JudgeType.Perfect;
        }
        else if (d <= _goodMs)
        {
            return JudgeType.Good;
        }
        return JudgeType.Miss;
    }
}

[Serializable]
public class GameScoreData
{
    public int Score;
    public int MaxCombo;
    public int CurrentCombo;
    public int Misses;
}