using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    [Header("BASE")]
    [SerializeField] protected bool _isShowing;
    
    protected CanvasGroup _canvasGroup;
    protected UIManager _uiManager;
    
    public CanvasGroup CanvasGroup => _canvasGroup;
    public UIManager UIManager => _uiManager;

    public virtual void OnInit(UIManager uiManager)
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _uiManager = uiManager;
        _isShowing = false;
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _isShowing = true;
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        _isShowing = false;
    }
}