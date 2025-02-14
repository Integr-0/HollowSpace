using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Door Tile")]
public class DoorTile : TileBase
{
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;

    [SerializeField] private GameObject interactionPrefab;

    private bool _isOpen = false;
    private BoxCollider2D _collider;
    
    private ITilemap _tilemap;
    private Vector3Int _position;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        _tilemap = tilemap;
        _position = position;
        
        if (!go) return false;
        
        //go.transform.position = position - new Vector3(0.5f, 0, 0);
        
        if (go.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(OnInteract);
        }
        
        _collider = go.GetComponent<BoxCollider2D>();
        
        UpdateTile();

        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = GetSprite();
        tileData.gameObject = interactionPrefab;
    }

    private Sprite GetSprite() {
        return _isOpen ? openedSprite : closedSprite;
    }

    private void OnInteract(InteractionAgent agent)
    {
        _isOpen = !_isOpen;
        UpdateTile();
    }

    private void UpdateTile() {
        // Update collider (can't disable the collider because it's needed for the interaction)
        _collider.isTrigger = _isOpen;
        
        // Update sprite
        _tilemap.RefreshTile(_position);
    }
}
