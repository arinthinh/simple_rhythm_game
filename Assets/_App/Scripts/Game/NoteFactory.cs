using UnityEngine;
using UnityEngine.Pool;

public class NoteFactory : MonoBehaviour
{
    [SerializeField] private RhythmNote _notePrefab;
    [SerializeField] private Transform[] _laneParents;
    
    private ObjectPool<RhythmNote> _notePool;

    private void Awake()
    {
        _notePool = new ObjectPool<RhythmNote>(
            createFunc: () => Instantiate(_notePrefab),
            actionOnGet: note => note.gameObject.SetActive(true),
            actionOnRelease: note => note.gameObject.SetActive(false),
            actionOnDestroy: note => Destroy(note.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );
    }
    
    public RhythmNote SpawnNote(int lane)
    {
        var note = _notePool.Get();
        var laneParent = _laneParents[Mathf.Clamp(lane, 0, _laneParents.Length - 1)];
        note.transform.SetParent(laneParent, false);
        return note;
    }

    public void ReleaseNote(RhythmNote note)
    {
        note.transform.SetParent(this.transform);
        _notePool.Release(note);
         
    }
}
 
