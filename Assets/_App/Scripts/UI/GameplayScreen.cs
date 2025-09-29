using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayScreen : UIScreen
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private TextMeshProUGUI _judgeText;

    public void ShowJudgeText(JudgeType judgeType)
    {
        // Change text
        _judgeText.text = judgeType switch
        {
            JudgeType.Perfect => "Perfect",
            JudgeType.Good => "Good",
            JudgeType.Miss => "Miss",
            _ => ""
        };
        
        // Change color
        var textColor = judgeType switch
        {
            JudgeType.Perfect => Color.yellow,
            JudgeType.Good => Color.cyan,
            JudgeType.Miss => Color.red,
            _ => Color.white
        };
        _judgeText.color = textColor;

        // Animate
        _judgeText.transform.DOKill();
        _judgeText.transform.localScale = Vector3.one * 0.5f;
        _judgeText.transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                _judgeText.transform.DOScale(0, 0.2f)
                    .SetDelay(0.5f)
                    .SetEase(Ease.InBack);
            });
    }

    public void UpdateScore(int score)
    {
        _scoreText.transform.DOKill();
        _scoreText.transform.localScale = Vector3.one;
        _scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f);
        _scoreText.text = $"{score}";
    }

    public void UpdateCombo(int combo)
    {
        if (combo > 2)
        {
            _comboText.transform.DOKill();
            _comboText.transform.localScale = Vector3.one;
            _comboText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f);
            _comboText.text = $"Combo x{combo}";
            _comboText.gameObject.SetActive(true);
        }
        else
        {
            _comboText.gameObject.SetActive(false);
        }
    }

    public void UpdateProgress(float progress)
    {
        _progressBar.fillAmount = progress;
    }
}