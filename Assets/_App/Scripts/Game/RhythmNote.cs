using System;
using DG.Tweening;
using UnityEngine;

public class RhythmNote : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visual;

    private double _targetTimeMs;
    private float _laneHeight = 6f;
    private float _leadMs = 1500f;
    private double _tUntil;

    public bool IsOutOfScreen => _tUntil < -100.0;
    public double TargetTimeMs => _targetTimeMs;

    public void Init(double targetTimeMs, float leadMs, float laneHeight)
    {
        // Initialize note parameters
        _targetTimeMs = targetTimeMs;
        _leadMs = leadMs;
        _laneHeight = laneHeight;
        _tUntil = leadMs;
        _visual.DOFade(1f, 0);

        // Move visual to start position
        var p = 1f - Mathf.Clamp01((float)((_leadMs - _tUntil) / _leadMs));
        var y = Mathf.Lerp(0f, _laneHeight, p);
        transform.localPosition = new Vector3(0f, y, 0f);
    }

    public void Move(double songTimeMsDSP)
    {
        // Update position based on current song time
        _tUntil = _targetTimeMs - songTimeMsDSP;
        var p = 1f - Mathf.Clamp01((float)((_leadMs - _tUntil) / _leadMs));
        var y = Mathf.Lerp(0f, _laneHeight, p);
        transform.localPosition = new Vector3(0f, y, 0f);
    }

    public void PlayDisappearAnimation(Action onComplete = null)
    {
        _visual.DOFade(0, 0.3f).OnComplete(() => { onComplete?.Invoke(); });
    }
}