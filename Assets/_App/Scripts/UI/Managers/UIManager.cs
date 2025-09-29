using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class UIManager : MonoSingleton<UIManager>
{
    [Header("UI CONTAINERS")]
    [SerializeField] private Transform _uiContainer;

    [Header("UI PREFABS")]
    [SerializeField] private UIScreen[] _screenPrefabs = Array.Empty<UIScreen>();

    private readonly Dictionary<string, UIScreen> _loadedScreens = new();

    #region Methods

    private UIScreen GetOrLoadScreen(Type type)
    {
        var screenName = type.FullName ?? string.Empty;
        if (_loadedScreens.TryGetValue(screenName, out var screen))
            return screen;

        for (int i = 0; i < _screenPrefabs.Length; i++)
        {
            var prefab = _screenPrefabs[i];
            if (prefab.GetType() == type)
            {
                var instance = Instantiate(prefab, _uiContainer);
                _loadedScreens.Add(screenName, instance);
                instance.OnInit(this);
                ReorderLoadedScreens();
                return instance;
            }
        }
        return null;
    }


    public T ShowScreen<T>() where T : UIScreen
    {
        var screen = GetOrLoadScreen(typeof(T)) as T;
        if (screen == null)
        {
            Debug.LogError("Invalid screen " + typeof(T).FullName);
            return null;
        }
        screen.Show();
        return screen;
    }

    public T HideScreen<T>() where T : UIScreen
    {
        var screen = GetOrLoadScreen(typeof(T)) as T;
        if (screen == null)
        {
            Debug.LogError("Invalid screen " + typeof(T).FullName);
            return null;
        }
        screen.Hide();
        return screen;
    }

    public T GetScreen<T>() where T : UIScreen
    {
        return GetOrLoadScreen(typeof(T)) as T;
    }

    public void ReleaseScreen<T>() where T : UIScreen
    {
        var screenName = typeof(T).FullName ?? string.Empty;
        if (_loadedScreens.TryGetValue(screenName, out var screen))
        {
            Destroy(screen.gameObject);
            _loadedScreens.Remove(screenName);
            ReorderLoadedScreens();
        }
    }

    // Ensures loaded screens are always in the correct order in the container
    private void ReorderLoadedScreens()
    {
        for (int i = 0; i < _screenPrefabs.Length; i++)
        {
            var prefab = _screenPrefabs[i];
            var screenName = prefab.GetType().FullName ?? string.Empty;
            if (_loadedScreens.TryGetValue(screenName, out var loadedScreen) && loadedScreen != null)
            {
                loadedScreen.transform.SetSiblingIndex(i);
            }
        }
    }
    
    #endregion
}