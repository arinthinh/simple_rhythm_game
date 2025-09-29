using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : UIScreen
{
    [SerializeField] private Button _playButton;

    public override void OnInit(UIManager uiManager)
    {
        base.OnInit(uiManager);
        _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        EventBus.Raise(new PlayButtonClickedEvent());
        Hide();
    }
}

public struct PlayButtonClickedEvent : IEvent
{
    
}
 
