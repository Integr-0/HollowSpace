using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class InteractableTile : TileBase {
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject interactionPrefab;
    
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!go) return false;
        
        if (go.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(OnInteract);
        }

        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.gameObject = interactionPrefab;
    }

    protected abstract void OnInteract(InteractionAgent agent);
}