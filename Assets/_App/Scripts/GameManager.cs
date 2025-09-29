using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public async UniTaskVoid Start()
    {
        await UniTask.Yield();
        UIManager.Instance.ShowScreen<HomeScreen>();
    }
}