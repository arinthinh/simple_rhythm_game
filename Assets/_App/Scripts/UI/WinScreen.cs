using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : UIScreen
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _maxComboText;

    private void OnEnable()
    {
        _replayButton.onClick.AddListener(OnReplayButtonClicked);
    }

    private void OnDisable()
    {
        _replayButton.onClick.RemoveListener(OnReplayButtonClicked);
    }

    public void SetData(int score, int maxCombo)
    {
        _scoreText.text = "Score:" + score;
        _maxComboText.text = "Max Combo:" + maxCombo;
    }

    private void OnReplayButtonClicked()
    {
        UIManager.Instance.ShowScreen<HomeScreen>();
        Hide();
    }
}